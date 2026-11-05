#!/bin/bash
set -euo pipefail
export LC_ALL=C

fail() { printf '%s\n' "$*" >&2; exit 1; }

if [[ $# == 1 && ( $1 == --help || $1 == -h ) ]]; then
    printf '%s\n' 'Usage: bash Tools/prepare-audio.sh [path to the original Biomes! Caverns mod]'
    exit 0
fi
[[ $# -le 1 ]] || fail 'Usage: bash Tools/prepare-audio.sh [path to the original Biomes! Caverns mod]'
[[ $(/usr/bin/uname -s) == Darwin ]] || fail 'This audio setup uses the macOS audio converter.'

mod=$(cd -P -- "$(dirname -- "$0")/.." && pwd)
source=${1:-"$HOME/Library/Application Support/Steam/steamapps/workshop/content/294100/2969748433"}
[[ -d "$source" ]] || fail "Biomes! Caverns is missing at $source. Pass the installed original mod folder as an argument."
source=$(cd -P -- "$source" && pwd)
package=$(/usr/bin/xmllint --nonet --xpath 'string(/ModMetaData/packageId)' "$source/About/About.xml")
[[ $(printf '%s' "$package" | /usr/bin/tr '[:upper:]' '[:lower:]') == biomesteam.biomescaverns ]] ||
    fail "This folder does not contain the original Biomes! Caverns mod: $source"

manifest="$mod/audio-manifest.tsv"
cache="$mod/AudioCache"
lock="$mod/.prepare-audio.lock"
stage=''
cleanup() {
    if [[ -n "$stage" && -d "$stage/previous" && ! -e "$cache" && ! -L "$cache" ]]; then
        if ! /bin/mv "$stage/previous" "$cache"; then
            printf 'Previous cache kept at %s/previous\n' "$stage" >&2
            /bin/rmdir "$lock"
            return
        fi
    fi
    [[ -z "$stage" ]] || /bin/rm -rf "$stage"
    /bin/rmdir "$lock"
}

[[ ! -L "$cache" && ( ! -e "$cache" || -d "$cache" ) ]] || fail "Expected a cache directory at $cache."
/bin/mkdir "$lock" || fail "Another audio setup may be running. Check $lock."
trap cleanup EXIT
trap 'exit 130' INT
trap 'exit 143' TERM
stage=$(/usr/bin/mktemp -d "$mod/.prepare-audio.XXXXXX")
/bin/cp "$manifest" "$stage/manifest.tsv"

# The published manifest is the sole inventory of input and output paths.
/usr/bin/awk -F '\t' '
    NF != 3 || $1 !~ /^Sounds\/BMT_Caverns\/[a-zA-Z0-9_.\/() -]+\.mp3$/ ||
    $1 ~ /(^|\/)\.{1,2}(\/|$)|\/\// || $2 != substr($1, 1, length($1) - 4) ".wav" ||
    length($3) != 64 || $3 ~ /[^0-9a-f]/ || seen[$1]++ { exit 1 }
    { print $3 "  " $1 }
    END { if (NR == 0) exit 1 }
' "$stage/manifest.tsv" > "$stage/originals.sha256" || fail 'The audio manifest is empty or invalid.'

verify_originals() {
    (cd -- "$source" && /usr/bin/shasum -a 256 --check --quiet --strict "$stage/originals.sha256") ||
        fail 'The installed audio differs from this release. Restore or update the original Biomes! Caverns files before preparing the cache.'
}
verify_originals

inputs=()
outputs=()
while IFS=$'\t' read -r input output _; do
    inputs+=("$input")
    outputs+=("$output")
done < "$stage/manifest.tsv"

cache_hashes() {
    (cd -- "$1" && /usr/bin/shasum -a 256 -- "${outputs[@]}")
}

if [[ -f "$cache/.manifest.tsv" && -f "$cache/.pcm.sha256" ]] &&
    /usr/bin/cmp -s "$stage/manifest.tsv" "$cache/.manifest.tsv" &&
    cache_hashes "$cache" 2>/dev/null | /usr/bin/cmp -s "$cache/.pcm.sha256" -; then
    printf 'Audio cache verified: %d clips.\n' "${#outputs[@]}"
    exit 0
fi

printf 'Preparing %d audio clips from your installed Biomes! Caverns files...\n' "${#outputs[@]}"
for ((i = 0; i < ${#outputs[@]}; i++)); do
    wav="$stage/next/${outputs[i]}"
    /bin/mkdir -p -- "${wav%/*}"
    /usr/bin/afconvert -f WAVE -d LEI16 "$source/${inputs[i]}" "$wav" || fail "Could not decode ${inputs[i]}"
    /usr/bin/afinfo "$wav" | /usr/bin/awk '
        /^File type ID:[[:space:]]+WAVE$/ { wave = 1 }
        /^Data format:.*Int16, interleaved$/ { pcm = 1 }
        /^audio bytes:[[:space:]]+[1-9][0-9]*$/ { data = 1 }
        END { if (!wave || !pcm || !data) exit 1 }
    ' || fail "Expected a nonempty 16-bit PCM WAV for ${inputs[i]}"
done
verify_originals
/usr/bin/cmp -s "$stage/manifest.tsv" "$manifest" || fail 'The mod updated while preparing its audio. Run the command again.'
cache_hashes "$stage/next" > "$stage/next/.pcm.sha256"
/bin/cp "$stage/manifest.tsv" "$stage/next/.manifest.tsv"

if [[ -d "$cache" ]]; then
    /bin/mv "$cache" "$stage/previous"
fi
/bin/mv "$stage/next" "$cache"
printf 'Audio cache ready: %d clips. Start RimWorld after enabling the required mods.\n' "${#outputs[@]}"

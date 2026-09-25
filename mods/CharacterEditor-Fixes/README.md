# Character Editor — Fixes [Star Sky Mods]

[![Character Editor — Fixes](https://raw.githubusercontent.com/chupakobra6/star-sky-assets/main/assets/character-editor-fixes.png)](https://steamcommunity.com/sharedfiles/filedetails/?id=3807830515)

[Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3807830515) · [All categories](../../README.md) · [Sources & notices](NOTICES.md)

Character Editor — Fixes возвращает гены размера тела в список редактора и исправляет чтение двух имён из стандартного файла Character Editor.

## Исправления

- Список генов размера тела использует идентификаторы генов. Перевод названий больше не скрывает эти гены в редакторе.
- При загрузке имён удаляется начальная метка кодировки и восстанавливается разделитель перед Kai Henriksen. Оба имени доступны отдельно.

## Установка

Установите [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077) и [Character Editor](https://steamcommunity.com/sharedfiles/filedetails/?id=1874644848). Включите их в порядке Harmony → Character Editor → Character Editor — Fixes. Требуется RimWorld 1.6.

В редакторе персонажа исправления действуют при показе списка генов и загрузке имён. Дополнительных настроек нет.

## Исходный код

Код находится в `Source/Fixes.cs`, собранная DLL — в `Assemblies/CharacterEditorFixes.dll`. Исправления применяются через Harmony к методам установленного Character Editor; файлы оригинального мода не входят в пакет. Условия распространения собственного кода — в [LICENSE](LICENSE), источники и уведомления — в [NOTICES.md](NOTICES.md).

## Обратная связь

<!-- rimmods:readme feedback begin -->
Заметили ошибку? Напишите в комментариях на странице мода или на Boosty.
<!-- rimmods:readme feedback end -->

<!-- rimmods:readme credit begin -->
Автор оригинального мода: void.
<!-- rimmods:readme credit end -->

## Уведомление Ludeon

<!-- rimmods:readme ludeon begin -->
Portions of the materials used to create this content/mod are trademarks and/or copyrighted works of Ludeon Studios Inc. All rights reserved by Ludeon. This content/mod is not official and is not endorsed by Ludeon.
<!-- rimmods:readme ludeon end -->

Folgendes Nuget-Package installieren
"Microsoft.EntityFrameworkCore.Design""
um Migrations durchführen zu können.

Migrations hinzufügen Beispiel für SQLite:
Add-Migration 1_Mig -Args "--provider Sqlite" -Context SqliteDataContext

Migration einspielen für SQLite:
Update-Database -Context SqliteDataContext

um die Lokale SQLite DB anschauen zu können empfiehlt sich der SQLite Browser
Download unter https://sqlitebrowser.org/
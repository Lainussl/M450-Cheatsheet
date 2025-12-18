# M450 Prüfung - Vorbereitung & Simulation

Dieses Repository enthält die komplette Vorbereitung für die M450 Prüfung "Applikationen testen".

## 📁 Struktur
- `docs/cheatsheet.md` - Praktisches Cheatsheet mit allen Mustern
- `src/LibraryManagement/` - Beispiel-Projekt (Bibliothek)
- `tests/LibraryManagement.Tests/` - Alle Unit-Tests mit MSTest2 + MOQ

## 🚀 Schnellstart
```bash
# Repository klonen
git clone https://github.com/deinname/m450-pruefung.git
cd m450-pruefung

# Solution öffnen
start M450.sln  # oder mit Rider/VS

# Alle Tests ausführen
dotnet test

# Nur Unit-Tests
dotnet test --filter UnitTests

# Nur Edge-Case-Tests
dotnet test --filter EdgeCaseTests
```

## 📚 Inhalt
- ✅ AAA-Pattern
- ✅ MSTest2 Assert-Methoden
- ✅ Eigene Test Doubles (Fake)
- ✅ MOQ Framework
- ✅ Äquivalenzklassen & Grenzwertanalyse
- ✅ Zustandsbasierte Tests
- ✅ Exception Testing
- ✅ Best Practices

## 🎯 Ziel
Nach diesem Repository kannst du:
- Unit-Tests mit MSTest2 schreiben
- Test Doubles (Fake, Mock) erstellen
- Äquivalenzklassen & Grenzwerte testen
- Zustandsbasierte Tests umsetzen
- Exceptions korrekt testen
- Clean Code Tests schreiben
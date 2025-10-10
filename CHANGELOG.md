# English:

## 🚀 WebKit 1.2.0 – What's New?

WebKit 1.2.0 introduces **XDSL** (Extensible Data Structure Language) support, giving developers more flexibility and usability when configuring their projects.

### ✨ Features
- **Dual Configuration Support**  
  Use either:
  - `webkit.json` (classic JSON format)  
  - `webkit.xdsl` (human-friendly XML-like format with comments)

- **Seamless Compatibility**  
  Both formats are interchangeable. WebKit automatically detects and loads the correct configuration.

- **Cleaner Syntax**  
  XDSL allows inline comments (`<!-- ... -->`) and structured declarations, making it easier to maintain complex configs.

### 📝 Example (`webkit.xdsl`)
```xml
<WebKit>
  <Properties>
    <Name>BosonWare</Name>
    <Description>The Official WebSite for BosonWare Technologies</Description>
    <Author>Realtin</Author>
    <URL>http://bosonware.org</URL>

    <!-- UI Assets -->
    <Logo>assets/logo.png</Logo>
    <Favicon>assets/favicon.png</Favicon>
    <Stylesheet>webkit.css</Stylesheet>
    <PhysicsBadge>@Shared/PhysicsBadge.html</PhysicsBadge>
    <Navbar>@Shared/Navbar.html</Navbar>
  </Properties>
</WebKit>
```


---

## WebKit 1.3.0 - What's New?
### Improvements
- Added conditional properties. Note: Supported only in `webkit.xdsl` configuration files.
- Improved Markdown to HTML conversion.
- Enhanced static site generation performance.
- Added new CLI options for customization.
- Fixed minor bugs and improved documentation.

### Breaking Changes:
- Changed default build directory structure.
- Changed `hot-reload` behavior: now requires `--watch` flag to enable live reloading.

_For more details, join the BosonWare Telegram community: https://t.me/BosonWare_



---



# Deutsch

## 🚀 WebKit 1.2.0 – Was ist neu?

Mit WebKit 1.2.0 kommt die Unterstützung für **XDSL** (Extensible Data Structure Language) und bietet mehr Flexibilität und Benutzerfreundlichkeit bei der Projektkonfiguration.

### ✨ Features
- **Duale Konfigurationsunterstützung**  
  Verwende entweder:
  - `webkit.json` (klassisches JSON-Format)  
  - `webkit.xdsl` (benutzerfreundliches XML-ähnliches Format mit Kommentaren)

- **Nahtlose Kompatibilität**  
  Beide Formate sind austauschbar. WebKit erkennt und lädt automatisch die richtige Konfiguration.

- **Saubere Syntax**  
  XDSL erlaubt Inline-Kommentare (`<!-- ... -->`) und klare Strukturen, ideal für komplexe Projekte.

### 📝 Beispiel (`webkit.xdsl`)
```xml
<WebKit>
  <Properties>
    <Name>BosonWare</Name>
    <Description>Die offizielle Website von BosonWare Technologies</Description>
    <Author>Realtin</Author>
    <URL>http://bosonware.org</URL>

    <!-- UI Assets -->
    <Logo>assets/logo.png</Logo>
    <Favicon>assets/favicon.png</Favicon>
    <Stylesheet>webkit.css</Stylesheet>
    <PhysicsBadge>@Shared/PhysicsBadge.html</PhysicsBadge>
    <Navbar>@Shared/Navbar.html</Navbar>
  </Properties>
</WebKit>
```

## WebKit 1.3.0 – Was ist neu?

### Verbesserungen
- Bedingte Eigenschaften hinzugefügt. Hinweis: Wird nur in `webkit.xdsl`-Konfigurationsdateien unterstützt.  
- Verbesserte Markdown-zu-HTML-Konvertierung.  
- Verbesserte Leistung bei der statischen Seitengenerierung.  
- Neue CLI-Optionen für mehr Anpassungsmöglichkeiten hinzugefügt.  
- Kleinere Fehler behoben und Dokumentation verbessert.  

### Breaking Changes:
- Standardverzeichnisstruktur für Builds geändert.  
- Verhalten von `hot-reload` geändert: Erfordert jetzt das Flag `--watch`, um Live-Reloading zu aktivieren.  

_Für weitere Details, tritt der BosonWare-Telegram-Community bei: https://t.me/BosonWare_

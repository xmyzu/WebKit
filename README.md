# WebKit

**WebKit** is a hybrid Markdown + HTML site engine written in **C#**.  
It transforms simple `.md` pages into clean, responsive websites — with built-in layouts, dark/light mode, and expression support.

---

## ✨ Features
- Responsive layout 📱
- Dark 🌙 + Light ☀️ mode
- Hybrid **Markdown + HTML** syntax
- Simple **expressions** with Getters + Setters
- Configurable via `webkit.json`

---

## 📦 Installation

### Using .NET Tool
```bash
dotnet tool install -g Boson.WebKit
```

---

## ⚡ Quick Start

```bash
webkit init -n MySite
cd MySite
webkit build
webkit serve
```

Open [http://localhost:3000](http://localhost:3000) 🎉

---

## ⚙️ Configuration

`webkit.json`

```json
{
  "Properties": {
    "Name": "MySite",
    "Author": "CodingBoson"
  }
}
```

Access with expressions:

```markdown
# Welcome to {{ .Name }} by {{ .Author }}
```

---

## 📂 Project Structure

```
MySite/
 ├─ build/                 # Generated output
 ├─ Resources/             # All resources live here
 │   ├─ Pages/             # Markdown + hybrid HTML pages
 │   ├─ Shared/            # Reusable components
 │   ├─ Static/            # CSS, JS, images
 │   └─ Layout.html        # Global layout
 ├─ .gitignore
 ├─ README.md
 └─ webkit.json            # Site config
```

---

## 🛠 Commands

```bash
webkit init <Name>     # Create new site
webkit build           # Build static site
webkit serve           # Run local dev server
webkit clean           # Clear build output
```

---

# Layout

The `Layout.html` in `Resources/` defines the global wrapper for your pages.  
Every page gets rendered **inside** this layout.

Example:
```html
<!DOCTYPE html>
<html>
<head>
  <title>{{ .Title }}</title>
  <link rel="stylesheet" href="/webkit.css">
</head>
<body>
  {{ .NavBar }}
  <main>
    {{ .Content }}
  </main>
  <footer>
    <p>© {{ .Name }} by {{ .Author }}</p>
  </footer>
</body>
</html>
````

---

## Shared Components

In WebKit, **shared components** are just **resources** inside `Resources/Shared/`.

To use them:

1. Create a file in `Resources/Shared/` (e.g. `NavBar.html`)
2. Reference it in `webkit.json`
3. Use it as a property in any page or layout

### Example

**`Resources/Shared/NavBar.html`**

```html
<nav>
  <a href="/">Home</a>
  <a href="/About.html">About</a>
</nav>
```

**`webkit.json`**

```json5
{
  "Properties": {
    "Name": "MySite",
    "Author": "CodingBoson",
    // First, define a property in the `webkit.json` that references the shared HTML/markdown file. "NavBar": "@Shared/NavBar.html"
    "NavBar": "@Shared/NavBar.html"
  }
}
```

**`Layout.html`**

```html
<body>
  {{ .NavBar }}
  <main>{{ .Content }}</main>
</body>
```

---

## Philosophy

* **No new syntax** → Components are just properties.
* **No hidden magic** → WebKit doesn’t treat `Shared/` specially. It’s just a folder convention.
* **Uniform API** → Whether you use `.Name`, `.Author`, or `.NavBar`, it’s the same expression system.

This makes WebKit:

* **Predictable** → All resources behave the same
* **Simple** → No separate “component language”
* **Composable** → You can nest and reuse shared parts freely

---

# 📑 Expressions

````markdown
# Expressions

WebKit supports **expressions** inside Markdown and HTML.  
They are written with double curly braces:

```markdown
{{ .Property }}
````

---

## 1. Getters

A **Getter** inserts the value of a property.

Example:

```markdown
Welcome to {{ .Name }} by {{ .Author }}
```

With this config:

```json
{
  "Properties": {
    "Name": "MySite",
    "Author": "CodingBoson"
  }
}
```

Result:

```html
Welcome to MySite by CodingBoson
```

---

## 2. Shared Resource References

Properties in `webkit.json` can point to other resources, like files in `Resources/Shared/`.

```json
{
  "Properties": {
    "NavBar": "@Shared/NavBar.html"
  }
}
```

Now you can use:

```markdown
{{ .NavBar }}
```

WebKit will inline the content of `Resources/Shared/NavBar.html`.

---

## 3. SetterExpressions

A **SetterExpression** allows you to define or override a property inside a page.

Syntax:

```markdown
{{ .Property = value }}
```

Example:

```markdown
{{ .Title = .Name Home }}

# Welcome to {{ .Name }}
```

Here:

* `.Title` is set to `"MySite Home"`
* It can be used later in `Layout.html` (e.g., inside `<title>`)

---

## 4. Concatenation in Setters

Setters can concatenate multiple values:

```markdown
{{ .Title = .Name " - " .Author }}
```

With:

```json
{
  "Properties": {
    "Name": "MySite",
    "Author": "CodingBoson"
  }
}
```

Result:

```html
<title>MySite - CodingBoson</title>
```

---

## 5. Resolution Order

When WebKit resolves an expression:

1. **Check runtime Setters** (defined in the current page)
2. **Check `webkit.json` Properties**
3. **If value is a resource reference** (e.g., `@Shared/...`), load its content
4. **Fallback** → leave the expression untouched

This makes expressions predictable:

* Pages can override defaults (via Setters)
* Layouts and Shared resources stay flexible

---

## 6. Escaping Expressions

To write an expression literally (without evaluating it), escape with `\\`:

```markdown
\\{{ .Author }}
```

Result:

```md
{{ .Author }}
```

---

## Philosophy

Expressions are the **glue of WebKit**:

* Getters → read values
* Setters → define or override values
* References → pull in resources
* All unified under one minimal syntax

👉 No custom DSL.
👉 No templates-within-templates.
👉 Just properties + resources, flowing through the build pipeline.

---

## 📜 License

GPL-3.0 © **BosonWare Technologies**

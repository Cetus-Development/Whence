# Whence Roadmap (draft)

> **Where does this value come from?**
>
> Whence records how values reach a property in a running .NET application
> and explains the path: which calls led there, which values were passed,
> and through which relationship (direct call, interface, callback, event).

## Why not just the call stack?

The call stack tells you **where** you are. Whence tells you **how you got there — and why.**

| Call stack                                           | Whence                                                                                               |
|------------------------------------------------------|------------------------------------------------------------------------------------------------------|
| One moment in time                                   | The **full history** — every change, over time                                                       |
| You have to find the right breakpoint first          | You name the **target**, Whence shows **every change it went through** — and how each one came about |
| No idea *why* this path was taken                    | The **branches** taken on the way (`if`/`else`, `switch`)                                            |
| Method names only                                    | The **values** passed at every step                                                                  |
| No idea *how* a method was reached                   | The **relationship**: field, event, callback — and its name                                          |
| Often ends in framework code at events and callbacks | Follows the chain **through** events and callbacks                                                   |

## v0.1 — first usable version

- **Command-line tool**, installed as a .NET global tool:
  `whence run [<app-folder>] --target <Class>.<Property>`
  - `<app-folder>` is the folder with the compiled application
    (e.g. `./bin/Debug/net8.0`); defaults to the current folder.
  - Whence finds the startup assembly by itself: it is the only one with a
    `<Name>.runtimeconfig.json` next to it. If there are several, Whence asks.
- **Never touches the original application.** Whence copies the compiled
  assemblies to a separate folder, adds recording calls to the copy only
  (via Mono.Cecil) and runs the copy. Source code and original files stay
  unchanged.
- **Instruments only the application's own assemblies.** External libraries
  are not instrumented; a value coming from them is shown as
  "came from `repository.GetAll()`".
- **Records every write** to the target property, **including collection
  changes** (e.g. `ObservableCollection.Add`, `Remove`, `Clear`) —
  many dropdowns are filled this way, not by setting the property.
- **For each write, records:**
  - the call chain that led to it (own code only)
  - the argument values — primitives, strings and `ToString()` of objects,
    with a depth limit
  - the relationship at each call site: direct call, interface/virtual call,
    callback (delegate) or event — including the **name** of the field,
    parameter or local variable the call goes through
  - the **branches taken** along the call chain (`if`/`else`, `switch`),
    with source line — this answers *why* the path was taken
- **Saves the recording to a file** first, then displays it.
  Recording and display stay separate, so a GUI can later read the same file.
- **Displays the history in the terminal in two levels** — like `git log` and `git show`:
  - `whence show` — the **history of the target**, one line per change:
    `#2 Clear`, `#3 Add "Andrew"`, …
  - `whence show <n>` — the **path to change `<n>`**: every step of the call chain
    with **file and line** (e.g. `Program.cs:7`), relationship, values and branches.
    Only the lines on the path are shown, not the code in between.
- **Does not crash** with multiple threads or `async`/`await`,
  even if the output is incomplete there.
- **Test target:** a sample console app that simulates a ViewModel scenario.

## Not in v0.1

- **Values inside conditions.** v0.1 shows *that* `if (customers.Count > 5)`
  took the `if` branch — a later version also shows *why*: `customers.Count` was `7`.
- Proper support for multiple threads and `async`/`await`
- Looking into external libraries
- Graphical UI or HTML report
- Attaching to an already running process
- WPF-specific features such as resolving bindings

## Ideas for later

- **Pick the target by clicking on a UI element** in the running application
- **Expand changes in place** in a GUI: the history stays visible, and each
  change can be expanded to show its path
- **Selectable views**, depending on how deep the user wants to go:
  - **Per change** (default): history first, path on demand
  - **Grouped by cause**: changes with the same path are grouped under it —
    a rough tree for large applications
- HTML report, timeline or graph view
- Jump from an entry straight to the source line
- Support for more application types: WPF, Avalonia, WinForms, console, web
- `whence run --project <path>`: build the project first, like `dotnet run`

## Principles

- Small releases, published early, improved based on feedback
- Start with the most common case, then add more step by step
- Recording and display are separate

## Decisions

- **Instrumented copy instead of a debugger** (for now). Faster and simpler;
  the original stays untouched. A debugger-based approach (nothing modified,
  can attach to running processes, but slow and complex) is reconsidered
  only if a concrete reason comes up.
- **.NET versions.** Whence itself is built with .NET 10. The recording
  library injected into the target targets .NET Standard 2.0, so target
  applications from **.NET 6 onward** are supported. v0.1 is tested with
  .NET 8 and .NET 10. .NET Framework may follow later.
- **How Whence runs the target.** `whence run [<app-folder>] --target <Class>.<Property> [-- <app args>]`:
  1. Copy the application folder to a separate working folder (e.g. `.whence/`).
  2. Detect the application's own assemblies via `MyApp.deps.json`
     (`"type": "project"` = own code, `"type": "package"` = NuGet package).
  3. Add recording calls to the own assemblies and place the recording library next to them.
  4. Start the copy. The user operates the application as usual; when it exits,
     Whence displays the recording.
- **Recording format: JSON Lines** (one JSON object per line, appended and
  flushed immediately). Whence may be used to find out *why* an application
  crashes — the recording must survive that crash. The recording library has
  **no dependencies** (it writes its JSON itself) to avoid version conflicts
  with the target application; the Whence CLI reads the file with
  `System.Text.Json`.

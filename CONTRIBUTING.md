# Contributing to Whence

Thanks for your interest! Issues, ideas and pull requests are welcome.

## Commit messages

Every commit follows this pattern:

```
type(scope): short summary in imperative mood

What changed?
- ...

Why?
- ...
```

**What changed?** describes the change. **Why?** explains the reason —
the part that is hardest to reconstruct later, so please don't skip it.

A template is included: run `git config commit.template .gitmessage` once,
and `git commit` will open it pre-filled.

### Example

```
fix(recorder): flush every line immediately

What changed?
- RecordingWriter now flushes after each JSON line

Why?
- Whence is often used to investigate crashes
- Buffered lines were lost when the target application crashed
```

### Types

| Type       | Use for                                                                     |
|------------|-----------------------------------------------------------------------------|
| `feat`     | New feature or functionality for the user                                   |
| `fix`      | Fixes a bug in existing behavior                                            |
| `refactor` | Restructuring without behavior change (e.g. split a method, rename a class) |
| `style`    | Purely cosmetic: formatting, indentation, whitespace — no logic change      |
| `docs`     | Documentation, comments, README                                             |
| `test`     | Adding, changing or fixing tests                                            |
| `perf`     | Improves performance without changing functionality                         |
| `build`    | Anything involved in building: `.csproj`, NuGet packages, SDK version       |
| `ci`       | CI/CD configuration (e.g. GitHub Actions)                                   |
| `chore`    | Other maintenance: `.gitignore`, editor settings, cleanup                   |
| `revert`   | Reverts a previous commit                                                   |

### Scopes

The scope names the part of Whence that is affected:

| Scope        | Area                                                       |
|--------------|------------------------------------------------------------|
| `cli`        | Commands and options (`whence run`, `--target`)            |
| `instrument` | Copying assemblies and adding recording calls (Mono.Cecil) |
| `recorder`   | The recording library inside the target application        |
| `format`     | The recording file format (JSON Lines)                     |
| `render`     | Displaying the recording in the terminal                   |
| `sample`     | Sample and test applications                               |

For `docs`, use the file name as scope, e.g. `docs(readme)` or `docs(roadmap)`.

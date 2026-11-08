# Whence

**Where does this value come from?**

Whence records how values reach a property in a running .NET application and
explains the path: which calls led there, which values were passed, which
branches were taken, and through which relationship — direct call, interface,
callback or event.

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

## Status

Early planning. Nothing to install yet — see the [roadmap](ROADMAP.md) for what's coming.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md).

## License

[MIT](LICENSE)

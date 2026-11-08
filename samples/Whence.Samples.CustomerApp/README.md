# Whence.Samples.CustomerApp

A small, made-up application that **Whence investigates**.

It recreates a typical situation from real desktop applications: a dropdown
shows a list of customers — but where and how does that list get filled?
Because we write this code ourselves, we know the answer, and we can check
whether Whence finds it.

Every feature planned for Whence v0.1 (see [ROADMAP.md](../../ROADMAP.md))
appears somewhere in this application. If Whence finds all of them,
v0.1 is complete.

## The story

- `CustomerViewModel` has a list `Customers` — what a dropdown would display.
- `Main` creates a repository with some test customers, the filter settings
  and the view model.
- `Main` then changes the filter twice. Each change raises an event; the view
  model reloads, filters the customers and fills the list.
- At the end, the application prints the list.

The question for Whence:

```
whence run --target CustomerViewModel.Customers
```

> Where do the entries in `CustomerViewModel.Customers` come from?

## What Whence must detect — and where

| Whence must detect …            | Where in this application                                  |
|---------------------------------|------------------------------------------------------------|
| Property being **set**          | `Customers = new ObservableCollection<Customer>()`         |
| Collection being **changed**    | `Customers.Add(...)`, `Customers.Clear()`                  |
| **Direct** call                 | `Reload()` calls `ApplyFilter()`                           |
| **Interface** call + field name | `_repository.GetAll()` via `ICustomerRepository`           |
| **Callback**                    | `_repository.ForEach(c => Customers.Add(c))`               |
| **Event**                       | `settings.FilterChanged += OnFilterChanged`                |
| **Branches**                    | `if (customer.IsActive)` and a `switch` on the filter mode |
| **Values** passed               | Customers, filter mode and parameters passed along         |
| Value from **external code**    | e.g. LINQ `.OrderBy(...)`                                  |
| `async` **without crashing**    | an `async` method that adds customers after an `await`     |

## Expected result

*To be written once the application exists:* what Whence should report for
`CustomerViewModel.Customers` — every change, its call chain, the values,
the relationships and the branches taken.

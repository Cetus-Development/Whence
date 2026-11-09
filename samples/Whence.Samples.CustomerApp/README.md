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

| Whence must detect …                 | Where in this application                                                                        |
|--------------------------------------|--------------------------------------------------------------------------------------------------|
| Property being **set**               | `Customers = new ObservableCollection<Customer>()`                                               |
| Collection being **changed**         | `Customers.Add(...)`, `Customers.Clear()`                                                        |
| **Direct** call                      | `Reload()` calls `ApplyFilter()`                                                                 |
| **Interface** call + field name      | `customerRepository.GetAll()` via `ICustomerRepository`                                          |
| **Callback**                         | `customerRepository.ForEach(c => Customers.Add(c))`                                              |
| **Event**                            | `_settings.FilterChanged += OnFilterChanged`                                                     |
| **Branches**                         | `switch (_mode)` and `if (customer.IsActive == wantActive)`                                      |
| **Values** passed                    | Customers, filter mode and parameters passed along                                               |
| Value from **external code**         | e.g. LINQ `.OrderBy(...)`                                                                        |
| `async` **without crashing**         | `LoadMoreAsync()` adds customers after `await customerRepository.GetCustomersAsync()`            |
| A value that **bypasses** the filter | `LoadMoreAsync()` adds "Nadia" (active) although the filter is `InactiveOnly` — a deliberate bug |

## Expected result

What Whence should report for `CustomerViewModel.Customers`, in two levels
(see the roadmap): first the history, then the path to each change.

### Level 1 — the history: `whence show`

```
#1   Set    Customers = new ObservableCollection<Customer>()
#2   Clear
#3   Add    Andrew   (active)
#4   Add    Lily     (active)
#5   Add    Arthur   (inactive)
#6   Add    Robin    (active)
#7   Add    Mathilda (inactive)
#8   Add    Maja     (inactive)
#9   Clear
#10  Add    Andrew   (active)
#11  Add    Lily     (active)
#12  Add    Robin    (active)
#13  Clear
#14  Add    Arthur   (inactive)
#15  Add    Maja     (inactive)
#16  Add    Mathilda (inactive)
#17  Add    Hans     (inactive)
#18  Add    Nadia    (active)
```

Final content: Arthur, Maja, Mathilda, Hans, Nadia — matches the console output.

### Level 2 — the path to a change: `whence show <n>`

Each path starts at the outermost call and ends at the change.
Only the lines on the path are shown.

**#1 — Set**

```
Program.cs:5              new CustomerViewModel(customerRepository, filterSettings)
└ CustomerViewModel.cs:16   Customers = new ObservableCollection<Customer>()
```

**#2 — Clear** (initial load)

```
Program.cs:7              customerVM.Reload()
└ CustomerViewModel.cs:28   Customers.Clear()
```

**#3 — Add Andrew** (initial load, via callback)

```
Program.cs:7              customerVM.Reload()
└ CustomerViewModel.cs:29   ApplyFilter(filterSettings.Filter)        value: All
  └ CustomerViewModel.cs:34   switch (_mode)                          branch: case All
    └ CustomerViewModel.cs:37   customerRepository.ForEach(...)       interface call via field customerRepository
      └ CustomerRepository.cs:22  action(customer)                    callback, call 1 of 6, value: Andrew
        └ CustomerViewModel.cs:37   c => Customers.Add(c)             lambda defined in ApplyFilter
```

#4 – #8 take the same path; only the callback call (2 – 6) and the value differ.

**#9 — Clear** (filter changed to `ActiveOnly`)

```
Program.cs:9              filterSettings.Filter = FilterMode.ActiveOnly
└ FilterSettings.cs:11      if (value != filter)                      branch: true (All → ActiveOnly)
  └ FilterSettings.cs:14      FilterChanged?.Invoke(value)            event, subscribed via _settings (CustomerViewModel.cs:18)
    └ CustomerViewModel.cs:23   Reload()                              in OnFilterChanged, value: ActiveOnly
      └ CustomerViewModel.cs:28   Customers.Clear()
```

**#10 — Add Andrew** (filter `ActiveOnly`)

```
Program.cs:9              filterSettings.Filter = FilterMode.ActiveOnly
└ FilterSettings.cs:11      if (value != filter)                      branch: true
  └ FilterSettings.cs:14      FilterChanged?.Invoke(value)            event via _settings
    └ CustomerViewModel.cs:23   Reload()
      └ CustomerViewModel.cs:29   ApplyFilter(filterSettings.Filter)  value: ActiveOnly
        └ CustomerViewModel.cs:34   switch (_mode)                    branch: case ActiveOnly
          └ CustomerViewModel.cs:44   customerRepository.GetAll().OrderBy(c => c.Name)
          │                           interface call via field customerRepository; OrderBy is external code
          └ CustomerViewModel.cs:46   if (customer.IsActive == wantActive)   branch: true (true == true)
            └ CustomerViewModel.cs:48   Customers.Add(customer)       value: Andrew
```

#11 – #12 take the same path with Lily and Robin.
Arthur, Maja and Mathilda are checked too, but `if` is false — no change, so no entry.

**#13 – #16** take the same paths as #9 – #12, starting at `Program.cs:10`
with `InactiveOnly` (`wantActive` = false).

**#18 — Add Nadia** (loaded asynchronously — the deliberate bug)

```
Program.cs:12             await customerVM.LoadMoreAsync()
└ CustomerViewModel.cs:61   await customerRepository.GetCustomersAsync()   interface call via field customerRepository
  │ └ CustomerRepository.cs:28  await Task.Delay(1000)
  │   ── execution continues after await, possibly on another thread ──
  └ CustomerViewModel.cs:65   Customers.Add(customer)               value: Nadia (active)
                              no filter branch on this path
```

This path answers the question *"How did an active customer get into a list
filtered to inactive ones?"*: it came through `LoadMoreAsync`, which never
checks the filter.

#17 (Hans) takes the same path.

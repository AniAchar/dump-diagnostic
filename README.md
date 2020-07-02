# dump-diagnostic

A CLI tool to run prebuilt analysis rules on dotnet core dumps on both Linux and Windows.

## Usage

`dump-diagnostic --dump-path <full path to the dump file> --diagnose <analysis> --report-type <report-type> --verbose`

### Valid diagnostic rules

```

- crash
- memory

```
### Valid report type

```

-console

```
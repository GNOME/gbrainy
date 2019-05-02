tools = [
  ['GameXmlToAssembly', files('GameXmlToAssembly.cs')],
  ['GameXmlToGetString', files('GameXmlToGetString.cs')],
]

sources = files(
  'GetTextParser/CatalogParser.cs',
  'GetTextParser/StringEscaping.cs',
  'TranslationsChecker.cs',
)

tools += [['TranslationsChecker', sources]]

foreach tool: tools
  executable(
    tool[0],
    tool[1],
    dependencies: libgbrainy_core_dep,
    cs_args: '-r:Mono.Posix',
  )
endforeach

sources = files('GamesStatistics.cs')

sources += configure_file(
  input: 'Defines.cs.in',
  output: '@BASENAME@',
  configuration: defines_conf,
)

game_statistics = executable(
  'GamesStatistics',
  sources,
  dependencies: libgbrainy_core_dep,
  cs_args: '-r:Mono.Posix',
)

run_target(
  'run-stats',
  command: [find_program('mono'), game_statistics, meson.current_source_dir()],
)

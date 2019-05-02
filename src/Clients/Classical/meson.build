sources = files(
  'CommandLine.cs',
  'Dialogs/AboutDialog.cs',
  'Dialogs/BuilderDialog.cs',
  'Dialogs/CustomGameDialog.cs',
  'Dialogs/HigMessageDialog.cs',
  'Dialogs/PdfExportDialog.cs',
  'Dialogs/PlayerHistoryDialog.cs',
  'Dialogs/PreferencesDialog.cs',
  'gbrainy.cs',
  'GtkSynchronize.cs',
  'Widgets/BrowseFile.cs',
  'Widgets/GameDrawingArea.cs',
  'Widgets/Toolbar.cs',
)

sources += configure_file(
  input: 'Defines.cs.in',
  output: '@BASENAME@',
  configuration: defines_conf,
)

sources += configure_file(
  input: 'AssemblyInfo.cs.in',
  output: '@BASENAME@',
  configuration: defines_conf,
)

cs_flags = [
  '-nowarn:0169',
  # FIXME: Language C sharp does not support library finding.
  '-r:Mono.Posix',
]

# FIXME: this should done with resx file and resources parameter
resource_data = [
  data_dir / 'app-graphics/resume-32.png',
  data_dir / 'app-graphics/endgame-32.png',
  data_dir / 'app-graphics/pause-32.png',
  data_dir / 'app-graphics/allgames-32.png',
  data_dir / 'app-graphics/gbrainy.png',
  data_dir / 'app-graphics/logic-games-32.png',
  data_dir / 'app-graphics/math-games-32.png',
  data_dir / 'app-graphics/memory-games-32.png',
  data_dir / 'app-graphics/verbal-games.svg',
  data_dir / 'app-graphics/verbal-games-32.png',
  data_dir / 'app-graphics/gbrainy.svg',
  data_dir / 'app-graphics/gtk-info.svg',
  data_dir / 'app-graphics/gtk-stop.svg',
  data_dir / 'app-graphics/gtk-ok.svg',
  meson.current_source_dir() / 'Dialogs/ui/CustomGameDialog.ui',
  meson.current_source_dir() / 'Dialogs/ui/PdfExportDialog.ui',
  meson.current_source_dir() / 'Dialogs/ui/PlayerHistoryDialog.ui',
  meson.current_source_dir() / 'Dialogs/ui/PreferencesDialog.ui',
  meson.current_source_dir() / 'gbrainy.ui',
]

foreach resource: resource_data
  cs_flags += ['-resource:' + resource]
endforeach

gbrainy = executable(
  meson.project_name(),
  sources,
  dependencies: libgbrainy_core_dep,
  cs_args: cs_flags,
  install: true,
  install_dir: gbrainy_pkglibdir,
)

config = 'gbrainy.exe.config'

install_data(
  config + '.in',
  install_dir: gbrainy_pkglibdir,
  rename: config,
)

bin_conf = {
  'exec_prefix': '${prefix}',
  'libdir': gbrainy_prefix / gbrainy_libdir,
  'prefix': gbrainy_prefix,
}

configure_file(
  input: 'gbrainy.in',
  output: '@BASENAME@',
  configuration: bin_conf,
  install_dir: gbrainy_bindir,
)

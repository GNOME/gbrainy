desktop = 'gbrainy.desktop'

custom_target(
  desktop,
  input: desktop + '.in',
  output: '@BASENAME@',
  command: intltool_desktop_cmd,
  install: true,
  install_dir: gbrainy_datadir / 'applications',
)

appdata = 'gbrainy.appdata.xml'

custom_target(
  appdata,
  input: appdata + '.in',
  output: '@BASENAME@',
  command: intltool_xml_cmd,
  install: true,
  install_dir: gbrainy_datadir / 'metainfo',
)

pixmap_data = files(
  'app-graphics/gbrainy.png',
  'app-graphics/gbrainy.svg',
  'app-graphics/gbrainy16.png',
  'app-graphics/gbrainy48.png',
)

install_data(
  pixmap_data,
  install_dir: gbrainy_datadir / 'pixmaps',
)

analogies_data = files(
  'game-graphics/box.svg',
  'game-graphics/clock.svg',
  'game-graphics/dartboard.svg',
  'game-graphics/faceboy1.svg',
  'game-graphics/faceboy2.svg',
  'game-graphics/faceboy3.svg',
  'game-graphics/facegirl1.svg',
  'game-graphics/facegirl2.svg',
  'game-graphics/facegirl3.svg',
  'game-graphics/family.svg',
  'game-graphics/father_son.svg',
  'game-graphics/horses_men.svg',
  'game-graphics/lever.svg',
  'game-graphics/money.svg',
  'game-graphics/password.svg',
  'game-graphics/people_table.svg',
  'game-graphics/tennis.svg',
  'games.xml',
  'verbal_analogies.xml',
)

themes_data = files(
  'themes/classic_background.svg',
  'themes/notebook_background.svg',
  'themes/themes.xml',
)

sounds_data = files(
  'sounds/right.oga',
  'sounds/wrong.oga',
)

install_data(
  analogies_data + themes_data + sounds_data,
  install_dir: gbrainy_datadir / 'games' / meson.project_name(),
)

# man file
install_man('gbrainy.6')

# FIXME: fix tree
install_data(
  'app-graphics/gbrainy16.png',
  install_dir: gbrainy_icondir / '16x16/apps',
  rename: 'gbrainy.png'
)

install_data(
  'app-graphics/gbrainy.png',
  install_dir: gbrainy_icondir / '32x32/apps',
)

install_data(
  'app-graphics/gbrainy48.png',
  install_dir: gbrainy_icondir / '48x48/apps',
  rename: 'gbrainy.png'
)

install_data(
  'app-graphics/gbrainy.svg',
  install_dir: gbrainy_icondir / 'scalable/apps',
)

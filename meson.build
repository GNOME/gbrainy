project(
  'gbrainy', 'cs',
  version: '2.4.0',
  license: 'GPL2',
  default_options: 'buildtype=debugoptimized',
  meson_version: '>= 0.50.0'
)

gbrainy_version = meson.project_version()
gbrainy_asm_version = gbrainy_version + '.0'

gbrainy_prefix = get_option('prefix')
gbrainy_bindir = get_option('bindir')
gbrainy_datadir = get_option('datadir')
gbrainy_libdir = get_option('libdir')

gbrainy_pkglibdir = gbrainy_libdir / meson.project_name()

gbrainy_icondir = gbrainy_datadir / 'icons/hicolor'

gbrainy_debug = get_option('buildtype').contains('debug')

gnome = import('gnome')
i18n = import('i18n')
pkg = import('pkgconfig')

source_root = meson.current_source_dir()

data_dir = source_root / 'data'
po_dir = source_root / 'po'

intltool_merge = find_program('intltool-merge')
intltool_cache = join_paths(po_dir, '.intltool-merge-cache')
intltool_desktop_cmd = [intltool_merge, '-d', '-u', '-c', intltool_cache, po_dir, '@INPUT@', '@OUTPUT@']
intltool_xml_cmd = [intltool_merge, '-x', '-u', '-c', intltool_cache, po_dir, '@INPUT@', '@OUTPUT@']

top_inc = include_directories('.')

defines_conf = {
  'ASSEMBLY_VERSION': gbrainy_asm_version,
  'COPYRIGHT': '(c) 2007-2019 Jordi Mas i Hernandez',
  'prefix': gbrainy_prefix,
  'VERSION': gbrainy_version,
}

common_flags = []

# Required libraries
gtk_sharp_dep = dependency('gtk-sharp-3.0', version: '>= 2.99.1')

# we only check the availability on the system
dependency('libcanberra-gtk3', version: '>= 0.26')
dependency('librsvg-2.0')

# FIXME: Language C sharp does not support library finding. `declare_dependency` only works with C/C++
#mono_csharp_dep = csc.find_library('Mono.CSharp.dll')
#mono_posix_dep = csc.find_library('Mono.Posix')

# check for various mono assemblies that we need.
mono_libdir = dependency('mono', version: '>= 4.0.0').get_pkgconfig_variable('libdir')
foreach assembly: ['Mono.Posix', 'Mono.CSharp']
  assembly_dll = assembly + '.dll'
  if run_command('test', '-e', mono_libdir / 'mono/4.5' / assembly_dll).returncode() != 0
    error('missing required mono 4.5 DLL: ' + assembly_dll)
  endif
endforeach

mono_addins_req_version = '>= 0.2.0'

enable_addins = get_option('addins')
mono_addins_dep = dependency('mono-addins', version: mono_addins_req_version, required: enable_addins)
mono_addins_gui_dep = dependency('mono-addins-gui', version: mono_addins_req_version, required: enable_addins)
mono_addins_setup_dep = dependency('mono-addins-setup', version: mono_addins_req_version, required: enable_addins)
have_addins = mono_addins_dep.found() and mono_addins_gui_dep.found() and mono_addins_setup_dep.found()

assert(have_addins or not enable_addins.enabled(), 'mono-addins was not found or is not up to date. Please install mono-addins ' + mono_addins_req_version + ', or disable mono-addins support by -Daddins=false')

if have_addins
  common_flags += '-d:MONO_ADDINS'
endif

# Debugging support
if gbrainy_debug
  common_flags += '-d:DEBUG'
endif

# NUnit required for (optional) unit tests
enable_tests = get_option('tests')
if enable_tests
  nunit_dep = dependency('nunit', version: '>= 2.4.7')
  nunit_console = find_program('nunit-console')
  mono_nunit_dep = dependency('mono-nunit', version: '>= 2.4')
endif

# Prologue
# FIXME: necessary?
#gnome_icon_theme_prefix = dependency('gnome-icon-theme').get_pkgconfig_variable('prefix')

add_project_arguments(common_flags, language: 'cs')

subdir('src')

if enable_tests
  subdir('tests')
endif

subdir('data')
subdir('tools')
subdir('po')
subdir('help')

meson.add_install_script(
  'meson_post_install.py',
  gbrainy_datadir,
)

output = '\nMono-addins:\t\t' + have_addins.to_string() + '\n'
output += 'NUnit:\t\t\t' + enable_tests.to_string() + '\n'
output += 'Debug enabled:\t\t' + gbrainy_debug.to_string() + '\n'
message(output)

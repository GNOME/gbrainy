sources = files(
  'Libraries/CairoContext.cs',
  'Libraries/CSharpCompiler.cs',
  'Libraries/GetText.cs',
  'Libraries/TranslationsCatalog.cs',
  'Libraries/SVGImage.cs',
  'Main/ArrayListIndicesRandom.cs',
  'Main/CairoContextEx.cs',
  'Main/ColorPalette.cs',
  'Main/GameAnswerCheckAttributes.cs',
  'Main/GameAnswer.cs',
  'Main/GameAnswerEventArgs.cs',
  'Main/Game.cs',
  'Main/GameDifficulty.cs',
  'Main/GameLocator.cs',
  'Main/GameManager.cs',
  'Main/GameSession.cs',
  'Main/GameSessionHistory.cs',
  'Main/GameSessionHistoryExtended.cs',
  'Main/GameSessionPlayList.cs',
  'Main/GameTips.cs',
  'Main/GameTypes.cs',
  'Main/IDrawable.cs',
  'Main/IDrawRequest.cs',
  'Main/IMouseEvent.cs',
  'Main/Memory.cs',
  'Main/PdfExporter.cs',
  'Main/PlayerHistory.cs',
  'Main/PlayerPersonalRecord.cs',
  'Main/Preferences.cs',
  'Main/PreferencesStorage.cs',
  'Main/Score.cs',
  'Main/Theme.cs',
  'Main/ThemeManager.cs',
  'Main/UpdateUIStateEventArgs.cs',
  'Main/Verbal/Analogies.cs',
  'Main/Verbal/AnalogiesFactory.cs',
  'Main/Verbal/AnalogiesMultipleOptions.cs',
  'Main/Verbal/AnalogiesPairOfWordsCompare.cs',
  'Main/Verbal/AnalogiesPairOfWordsOptions.cs',
  'Main/Verbal/AnalogiesQuestionAnswer.cs',
  'Main/Verbal/Analogy.cs',
  'Main/Xml/DrawingObject.cs',
  'Main/Xml/ImageDrawingObject.cs',
  'Main/Xml/LocalizableString.cs',
  'Main/Xml/GameXml.cs',
  'Main/Xml/GameXmlDrawing.cs',
  'Main/Xml/GameXmlDefinition.cs',
  'Main/Xml/GameXmlDefinitionVariant.cs',
  'Main/Xml/GameXmlFactory.cs',
  'Main/Xml/OptionDrawingObject.cs',
  'Main/Xml/TextDrawingObject.cs',
  'Platform/Unix.cs',
  'Services/DefaultServices.cs',
  'Services/IConfiguration.cs',
  'Services/ICSharpCompiler.cs',
  'Services/IService.cs',
  'Services/ITranslations.cs',
  'Services/MemoryConfiguration.cs',
  'Services/ServiceLocator.cs',
  'Toolkit/Container.cs',
  'Toolkit/DrawableArea.cs',
  'Toolkit/DrawEventArgs.cs',
  'Toolkit/HorizontalContainer.cs',
  'Toolkit/SelectedEventArgs.cs',
  'Toolkit/Widget.cs',
  'Views/CountDownView.cs',
  'Views/FinishView.cs',
  'Views/PlayerHistoryView.cs',
  'Views/ViewsControler.cs',
  'Views/WelcomeView.cs',
)

sources += configure_file(
  input: 'Main/Defines.cs.in',
  output: '@BASENAME@',
  configuration: defines_conf,
)

sources += configure_file(
  input: 'AssemblyInfo.cs.in',
  output: '@BASENAME@',
  configuration: defines_conf,
)

deps = [gtk_sharp_dep]

cs_flags = [
  # Active when designing games
  #'-D:DESIGN_MODE',
  # FIXME: Language C sharp does not support library finding.
  '-r:Mono.CSharp',
  '-r:Mono.Posix',
]

# FIXME: this should done with resx file and resources parameter
resource_data = [
  data_dir / 'app-graphics/logic-games.svg',
  data_dir / 'app-graphics/math-games.svg',
  data_dir / 'app-graphics/memory-games.svg',
  data_dir / 'app-graphics/verbal-games.svg',
  meson.current_source_dir() / 'gbrainy.addin.xml',
]

foreach resource: resource_data
  cs_flags += ['-resource:' + resource]
endforeach

if have_addins
  deps += [
    mono_addins_dep,
    mono_addins_gui_dep,
    mono_addins_setup_dep,
  ]
endif

libgbrainy_core = shared_library(
  meson.project_name() + '.Core',
  sources: sources,
  dependencies: deps,
  cs_args: cs_flags,
  install: true,
  install_dir: gbrainy_pkglibdir,
)

libgbrainy_core_dep = declare_dependency(
  dependencies: deps,
  link_with: libgbrainy_core,
)

pkg.generate(
  libgbrainy_core,
  filebase: meson.project_name(),
  name: meson.project_name(),
  description: 'Play games that challenge your logic, verbal, calculation and memory abilities',
  libraries: ['-r:Mono.Cairo', '-r:Mono.Posix'],
  variables: 'exec_prefix=${prefix}',
  install_dir: gbrainy_libdir / 'pkgconfig',
)

config = 'gbrainy.Core.dll.config'

install_data(
  config + '.in',
  install_dir: gbrainy_pkglibdir,
  rename: config,
)

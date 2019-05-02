sources = files(
  'Core/AnalogiesFactoryTest.cs',
  'Core/GameAnswerTest.cs',
  'Core/GameManagerTest.cs',
  'Core/GameSessionTest.cs',
  'Core/GameSessionPlayListTest.cs',
  'Core/GameXmlFactoryTest.cs',
  'Core/PlayerHistoryTest.cs',
  'Core/PlayerPersonalRecordTest.cs',
  'Core/PreferencesTest.cs',
  'TestSupport/UnitTestSupport.cs',
)

deps = [
  libgbrainy_core_dep,
  nunit_dep,
]

unit_tests = [[meson.project_name() + '.Core.Test', {'sources': sources, 'dependencies': deps}]]

sources = files(
  'Clients/Classical/CommandLineTest.cs',
  'Clients/Classical/gbrainyTest.cs',
  'TestSupport/UnitTestSupport.cs',
)

deps = [
  libgbrainy_core_dep,
  nunit_dep,
]

unit_tests += [[meson.project_name() + '.ClassicalClient.Test', {'sources': sources, 'dependencies': deps}]]

sources = files(
  'TestSupport/UnitTestSupport.cs',
  'Games/Logic/PuzzleNumericRelationTest.cs',
)

deps = [
  libgbrainy_games_dep,
  nunit_dep,
]

unit_tests += [[meson.project_name() + '.Games.Test', {'sources': sources, 'dependencies': deps}]]

test_env = environment()
test_env.set('MONO_PATH', src_build_dir)

# FIXME: tests don't work due to issues in paths
foreach unit_test: unit_tests
  lib = shared_library(
    unit_test[0],
    objects: gbrainy.extract_all_objects(),
    cs_args: '-r:' + gbrainy.full_path(),
    kwargs: unit_test[1],
  )

  test(
    unit_test[0],
    nunit_console,
    args: [lib.full_path(), '-nologo'],
    env: test_env,
  )
endforeach

pixmapdir = $(datadir)/pixmaps
pixmap_DATA = app-graphics/gbrainy.png \
		app-graphics/gbrainy.svg \
		app-graphics/gbrainy16.png \
		app-graphics/gbrainy48.png

analogiesdir = $(datadir)/games/gbrainy
themesdir = $(datadir)/games/gbrainy
soundsdir = $(datadir)/games/gbrainy

hicolordir = $(datadir)/icons/hicolor
datafiles = $(datadir)/games/gbrainy

man_MANS = gbrainy.6

desktop_in_files=	\
	gbrainy.desktop.in	

desktop_files=$(desktop_in_files:.desktop.in=.desktop)

Applicationsdir = $(datadir)/applications
Applications_DATA = $(desktop_files)

pkgconfigdir = $(libdir)/pkgconfig
pkgconfig_DATA = gbrainy.pc

appdatadir = $(datadir)/metainfo
appdata_in_files = gbrainy.appdata.xml.in
appdata_DATA = $(appdata_in_files:.xml.in=.xml)

tango_icons = 				\
	app-graphics/gbrainy.png	\
	app-graphics/gbrainy.svg	\
	app-graphics/math-games-32.png	\
	app-graphics/logic-games-32.png	\
	app-graphics/memory-games-32.png	\
	app-graphics/resume-32.png		\
	app-graphics/endgame-32.png		\
	app-graphics/allgames-32.png		\
	app-graphics/pause-32.png \
	app-graphics/verbal-games-32.png \
	app-graphics/gtk-info.svg \
	app-graphics/gtk-stop.svg \
	app-graphics/gtk-ok.svg


analogies_DATA = \
	verbal_analogies.xml \
	games.xml \
	game-graphics/clock.svg \
	game-graphics/family.svg \
	game-graphics/password.svg \
	game-graphics/tennis.svg \
	game-graphics/father_son.svg \
	game-graphics/money.svg \
	game-graphics/box.svg \
	game-graphics/dartboard.svg \
	game-graphics/horses_men.svg \
	game-graphics/lever.svg \
	game-graphics/people_table.svg \
	game-graphics/faceboy1.svg \
	game-graphics/faceboy2.svg \
	game-graphics/faceboy3.svg \
	game-graphics/facegirl1.svg \
	game-graphics/facegirl2.svg \
	game-graphics/facegirl3.svg


themes_DATA = \
	themes/themes.xml \
	themes/classic_background.svg \
	themes/notebook_background.svg

sounds_DATA = \
	sounds/right.oga \
	sounds/wrong.oga


install-data-local:
	@-$(mkinstalldirs) $(DESTDIR)$(hicolordir)/scalable/apps
	$(INSTALL_DATA) $(srcdir)/app-graphics/gbrainy.svg $(DESTDIR)$(hicolordir)/scalable/apps/gbrainy.svg
	@-$(mkinstalldirs) $(DESTDIR)$(hicolordir)/16x16/apps
	$(INSTALL_DATA) $(srcdir)/app-graphics/gbrainy16.png $(DESTDIR)$(hicolordir)/16x16/apps/gbrainy.png
	@-$(mkinstalldirs) $(DESTDIR)$(hicolordir)/32x32/apps
	$(INSTALL_DATA) $(srcdir)/app-graphics/gbrainy.png $(DESTDIR)$(hicolordir)/32x32/apps/gbrainy.png
	@-$(mkinstalldirs) $(DESTDIR)$(hicolordir)/48x48/apps
	$(INSTALL_DATA) $(srcdir)/app-graphics/gbrainy48.png $(DESTDIR)$(hicolordir)/48x48/apps/gbrainy.png
	@-$(mkinstalldirs) $(DESTDIR)$(datafiles)
	$(INSTALL_DATA) $(srcdir)/verbal_analogies.xml $(DESTDIR)$(datafiles)/verbal_analogies.xml
	$(INSTALL_DATA) $(srcdir)/games.xml $(DESTDIR)$(datafiles)/games.xml
	$(INSTALL_DATA) $(srcdir)/game-graphics/clock.svg $(DESTDIR)$(datafiles)/clock.svg
	$(INSTALL_DATA) $(srcdir)/game-graphics/family.svg $(DESTDIR)$(datafiles)/family.svg
	$(INSTALL_DATA) $(srcdir)/game-graphics/password.svg $(DESTDIR)$(datafiles)/password.svg
	$(INSTALL_DATA) $(srcdir)/game-graphics/tennis.svg $(DESTDIR)$(datafiles)/tennis.svg
	$(INSTALL_DATA) $(srcdir)/game-graphics/father_son.svg $(DESTDIR)$(datafiles)/father_son.svg
	$(INSTALL_DATA) $(srcdir)/game-graphics/money.svg $(DESTDIR)$(datafiles)/money.svg
	$(INSTALL_DATA) $(srcdir)/game-graphics/box.svg $(DESTDIR)$(datafiles)/box.svg
	$(INSTALL_DATA) $(srcdir)/game-graphics/dartboard.svg $(DESTDIR)$(datafiles)/dartboard.svg
	$(INSTALL_DATA) $(srcdir)/game-graphics/horses_men.svg $(DESTDIR)$(datafiles)/horses_men.svg
	$(INSTALL_DATA) $(srcdir)/sounds/right.oga $(DESTDIR)$(datafiles)/right.oga
	$(INSTALL_DATA) $(srcdir)/sounds/wrong.oga $(DESTDIR)$(datafiles)/wrong.oga


gtk_update_icon_cache = gtk-update-icon-cache -f -t $(datadir)/icons/hicolor

install-data-hook:
	@-if test -z "$(DESTDIR)"; then \
		echo "Updating Gtk icon cache."; \
		$(gtk_update_icon_cache); \
	else \
		echo "*** Icon cache not updated.  After install, run this:"; \
		echo "***   $(gtk_update_icon_cache)"; \
	fi

uninstall-hook:
	rm -f $(DESTDIR)$(datafiles)/verbal_analogies.xml
	rm -f $(DESTDIR)$(hicolordir)/48x48/apps/gbrainy.png
	rm -f $(DESTDIR)$(hicolordir)/32x32/apps/gbrainy.png
	rm -f $(DESTDIR)$(hicolordir)/16x16/apps/gbrainy.png
	rm -f $(DESTDIR)$(hicolordir)/scalable/apps/gbrainy.svg
	$(gtk_update_icon_cache);

noinst_DATA = 	

DISTCLEANFILES = $(desktop_files) $(appdata_DATA)

EXTRA_DIST = $(pixmap_DATA) $(tango_icons) $(man_MANS) $(analogies_DATA) $(desktop_in_files) $(themes_DATA) $(sounds_DATA) gbrainy.pc.in $(appdata_in_files)

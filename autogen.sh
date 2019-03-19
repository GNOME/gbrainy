
error () {
	echo "Error: $1" 1>&2
	exit 1
}

check_autotool_version () {
	which $1 &>/dev/null || {
		error "$1 is not installed, and is required to configure $PACKAGE"
	}

	version=$($1 --version | head -n 1 | cut -f4 -d' ')
	major=$(echo $version | cut -f1 -d.)
	minor=$(echo $version | cut -f2 -d.)
	rev=$(echo $version | cut -f3 -d.)
	major_check=$(echo $2 | cut -f1 -d.)
	minor_check=$(echo $2 | cut -f2 -d.)
	rev_check=$(echo $2 | cut -f3 -d.)

	if [ $major -lt $major_check ]; then
		do_bail=yes
	elif [ $minor -lt $minor_check ] && [ $major = $major_check ]; then
		do_bail=yes
	elif [ x"$rev_check" != x"" ] && [ $rev -lt $rev_check ] && [ $minor = $minor_check ] && [ $major = $major_check ]; then
		do_bail=yes
	fi

	if [ x"$do_bail" = x"yes" ]; then
		error "$1 version $2 or better is required to configure $PROJECT"
	fi
}


srcdir=`dirname $0`
test -z "$srcdir" && srcdir=.

ORIGDIR=`pwd`
cd $srcdir
PROJECT=gbrainy
TEST_TYPE=-f
FILE=src/Makefile.am

DIE=0

(autoconf --version) < /dev/null > /dev/null 2>&1 || {
	echo
	echo "You must have autoconf installed to compile $PROJECT."
	echo "Download the appropriate package for your distribution,"
	echo "or get the source tarball at ftp://ftp.gnu.org/pub/gnu/"
	DIE=1
}

AUTOMAKE=automake
ACLOCAL=aclocal

check_autotool_version $ACLOCAL 1.10
check_autotool_version $AUTOMAKE 1.10

(grep "^AM_PROG_LIBTOOL" configure.ac >/dev/null) && {
  (libtool --version) < /dev/null > /dev/null 2>&1 || {
    echo
    echo "**Error**: You must have \`libtool' installed to compile $PROJECT."
    echo "Get ftp://ftp.gnu.org/pub/gnu/libtool-1.2d.tar.gz"
    echo "(or a newer version if it is available)"
    DIE=1
  }
}

(which yelp-build)|| {
	echo "**Error**: You must have yelp-tools installed to compile $PROJECT."
	DIE=1
}

if test "$DIE" -eq 1; then
	exit 1
fi

test $TEST_TYPE $FILE || {
	echo "You must run this script in the top-level $PROJECT directory"
	exit 1
}

if test -z "$*"; then
	echo "I am going to run ./configure with no arguments - if you wish "
        echo "to pass any to it, please specify them on the $0 command line."
fi

case $CC in
*xlc | *xlc\ * | *lcc | *lcc\ *) am_opt=--include-deps;;
esac

echo "Running aclocal..."
aclocal || exit 1
echo "Running autoconf..."
autoconf || exit 1
echo "Running autoheader..."
autoheader || exit 1
echo "Running automake..."
automake --gnu --add-missing --copy || exit 1

conf_flags="--enable-maintainer-mode --enable-compile-warnings" #--enable-iso-c

cd "$ORIGDIR"

if test x$NOCONFIGURE = x; then
  echo Running $srcdir/configure $conf_flags "$@" ...
  $srcdir/configure $conf_flags "$@" \
  && echo Now type \`make\' to compile $PROJECT  || exit 1
else
  echo Skipping configure process.
fi

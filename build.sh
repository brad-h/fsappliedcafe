#!/bin/bash
url="https://github.com/fsprojects/Paket"
if [ ! -d ".paket" ]; then
  mkdir .paket
  curl -L "$url/releases/download/3.13.3/paket.bootstrapper.exe" -o .paket/paket.bootstrapper.exe
fi

if test "$OS" = "Windows_NT"
then # For Windows
  .paket/paket.bootstrapper.exe
  exit_code=$?
  if [ $exit_code -ne 0 ]; then
    exit $exit_code
  fi
  packages/FAKE/tools/FAKE.exe $@ --fsiargs build.fsx
else # For Non Windows
  mono .paket/paket.bootstrapper.exe
  exit_code=$?
  if [ $exit_code -ne 0 ]; then
    exit $exit_code
  fi
  mono packages/FAKE/tools/FAKE.exe $@ --fsiargs build.fsx
fi

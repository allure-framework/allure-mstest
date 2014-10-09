MSTestAllureAdapter
===================

[![Build Status](https://api.travis-ci.org/someuser77/MSTestAllureAdapter.svg?branch=master)](https://travis-ci.org/someuser77/MSTestAllureAdapter)

An MSTestAllureAdapter.

Work in progress.

Usage: 
MSTestAllureAdapter.Console.exe <TRX file> [output target dir]

If '[output target dir]' is missing the reslts are saved in the current directory in a folder named 'results'.


This will generate the xml files from which the allure report can be created.

To generate a report using [allure-cli](https://github.com/allure-framework/allure-cli/releases/tag/allure-cli-2.1) use: "allure generate report-results -v 1.4.0"



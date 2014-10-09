MSTestAllureAdapter
===================

[![Build Status](https://api.travis-ci.org/someuser77/MSTestAllureAdapter.svg?branch=master)](https://travis-ci.org/someuser77/MSTestAllureAdapter)

**MSTestAllureAdapter** allows you convert an MSTest TRX file to the XMLs from which an Allure report can be generated.

It is a .NET/Mono based console application.


# Usage
```bash
MSTestAllureAdapter.Console.exe &lt; TRX file &gt; [output target dir]
```

If '[output target dir]' is missing the reslts are saved in the current directory in a folder named 'results'.

```bash
$ mono MSTestAllureAdapter.Console.exe sample.trx 
```

This will generate the xml files from which the allure report can be created based upon the 'sample.trx' file.

```bash
$ mono MSTestAllureAdapter.Console.exe sample.trx output-xmls
```

This will generate the xml files from which the allure report can be created in a folder named 'output-xmls' based upon the 'sample.trx' file.

If the target directory does not exists it is created.



To generate a report using [allure-cli](https://github.com/allure-framework/allure-cli/releases/tag/allure-cli-2.1): 
```bash
$ allure generate output-xmls -v 1.4.0
```

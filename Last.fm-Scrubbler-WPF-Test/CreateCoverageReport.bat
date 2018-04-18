IF not exist "bin\Debug\CoverageReport" mkdir "bin\Debug\CoverageReport"
Coverage\OpenCover\OpenCover.Console.exe -register:user -target:"bin\Debug\Last.fm-Scrubbler-WPF-Test.exe" -output:"bin\Debug\CoverageReport\CoverageResult.xml" -filter:"+[Last*]Last*" -log:Error
Coverage\ReportGenerator\ReportGenerator.exe -reports:"bin\Debug\CoverageReport\CoverageResult.xml" -targetdir:"bin\Debug\CoverageReport" -reporttypes:Html -verbosity:Error
start "" "bin\Debug\CoverageReport\index.htm"
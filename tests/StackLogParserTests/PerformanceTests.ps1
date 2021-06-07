$baseUri = "https://localhost:5001/"

# test the 15 line file
$test_1_time = Measure-Command -Expression {$test_1_result = Invoke-RestMethod -Uri "$($baseUri)Report?LogFile=%2FUsers%2Fwill%2FDownloads%2Fsre-log-1-main%2F15_line_example.log&LookupIpAddress=192.168.117.221&LookupUserAgent=badbot&LookupUserAgentMethod=GET&ByteAverageWindowStart=2020-04-23%2021%3A00%3A00Z&ByteAverageWindowEnd=2020-04-23%2021%3A59%3A59Z" -Method GET -ContentType "application/json"}
"Processing 15 lines took: $($test_1_time.TotalMilliseconds)ms" | Out-Host
$test_1_result | Out-Host

# test the 1mb file
$test_2_time = Measure-Command -Expression {$test_2_result = Invoke-RestMethod -Uri "$($baseUri)Report?LogFile=%2FUsers%2Fwill%2FDownloads%2Fsre-log-1-main%2F1_mb_example.log&LookupIpAddress=192.168.117.221&LookupUserAgent=badbot&LookupUserAgentMethod=GET&ByteAverageWindowStart=2020-04-23%2021%3A00%3A00Z&ByteAverageWindowEnd=2020-04-23%2021%3A59%3A59Z" -Method GET -ContentType "application/json"}
"Processing 1mb file took: $($test_2_time.TotalMilliseconds)ms" | Out-Host
$test_2_result | Out-Host

# test the 1gb file
$test_3_time = Measure-Command -Expression {$test_3_result =Invoke-RestMethod -Uri "$($baseUri)Report?LogFile=%2FUsers%2Fwill%2FDownloads%2Fsre-log-1-main%2F1_gb_example.log&LookupIpAddress=192.168.117.221&LookupUserAgent=badbot&LookupUserAgentMethod=GET&ByteAverageWindowStart=2020-04-23%2021%3A00%3A00Z&ByteAverageWindowEnd=2020-04-23%2021%3A59%3A59Z" -Method GET -ContentType "application/json"}
"Processing 1 gb file took: $($test_3_time.TotalMilliseconds)ms" | Out-Host
$test_3_result | Out-Host
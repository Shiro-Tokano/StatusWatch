param(
    [string]$Root = (Get-Location),
    [string]$OutFile = "all-files.txt",
    [string[]]$IncludeExt = @(
        "cs","csproj","sln","json","xml","config","props","targets",
        "cshtml","razor","html","css","scss","js","ts",
        "md","txt","yml","yaml"
    )
)

# очистка выходного файла
if (Test-Path $OutFile) { Remove-Item $OutFile -Force }

# выбор файлов
$files = Get-ChildItem -Path $Root -Recurse -File -Force |
    Where-Object {
        $_.FullName -notmatch '\\bin\\|\\obj\\|\\\.git\\|\\\.vs\\|\\node_modules\\|\\packages\\' -and
        ($IncludeExt -contains $_.Extension.TrimStart('.').ToLower())
    }

foreach ($f in $files) {
    $content = Get-Content -Path $f.FullName -Raw -ErrorAction SilentlyContinue
    Add-Content -Path $OutFile -Encoding UTF8 -Value @"
$($f.FullName)
$content
---END---
"@
}

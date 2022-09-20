ClientReport_ and fileType = "Report Builder Report File" = Used to open the report in Report Builder and the extension is rdl.

filetype = Report Definition file (ext is .rdlc) = Is used in VS.

Open .rdlc file in VS & do the following to generate report.
Source: https://stackoverflow.com/questions/38902037/ssrs-report-definition-is-newer-than-server?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa

1. Remove MustUnderstand="df"
2. Change the xmlns value "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition" to "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition".
3. Delete the xmlns:df attribute (<df:DefaultFontFamily>Segoe UI</df:DefaultFontFamily>).
4. Delete the "ReportSections" and "ReportSection" opening and closing tags (not the content).
5. Delete the entire "ReportParametersLayout" block.
6. Delete the "df" tag and its content, if any.

Note: All field name in SP should be lower-case.
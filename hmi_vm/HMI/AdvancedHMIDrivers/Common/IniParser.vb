Imports System.IO
Imports System.Collections
'*************************************************************************************
'* Adapted from http://bytes.com/topic/net/insights/797169-reading-parsing-ini-file-c
'*************************************************************************************
Public Class IniParser
    Private keyPairs As New Hashtable()
    Private iniFilePath As String

    Private Structure SectionPair
        Public Section As String
        Public Key As String
    End Structure

    ''' <summary>
    ''' Opens the INI file at the given path and enumerates the values in the IniParser.
    ''' </summary>
    ''' <param name="iniPath">Full path to INI file.</param>
    Public Sub New(iniPath As String)
        'Dim iniFile As TextReader = Nothing
        Dim strLine As String = Nothing
        Dim currentRoot As String = Nothing
        Dim keyPair As String() = Nothing

        iniFilePath = iniPath

        If File.Exists(iniPath) Then
            Try
                Using iniFile As New StreamReader(iniPath)

                    strLine = iniFile.ReadLine()

                    While strLine IsNot Nothing
                        strLine = strLine.Trim()  '.ToUpper()

                        If Not String.IsNullOrEmpty(strLine) Then
                            If strLine.StartsWith("[") AndAlso strLine.EndsWith("]") Then
                                currentRoot = strLine.Substring(1, strLine.Length - 2).ToUpper
                            Else
                                keyPair = strLine.Split(New Char() {"="c}, 2)

                                Dim sectionPair As New SectionPair
                                Dim value As String = Nothing

                                If currentRoot Is Nothing Then
                                    currentRoot = "ROOT"
                                End If

                                sectionPair.Section = currentRoot
                                sectionPair.Key = keyPair(0).ToUpper

                                If keyPair.Length > 1 Then
                                    value = keyPair(1)
                                End If

                                Try
                                    keyPairs.Add(sectionPair, value)
                                Catch ex As Exception
                                    '* TODO: If a duplicate key exists, it will throw and exception
                                    Dim dbg = 0
                                End Try
                            End If
                        End If

                        strLine = iniFile.ReadLine()

                    End While
                End Using
            Catch ex As Exception
                Throw 'ex
                'Finally
                '    If iniFile IsNot Nothing Then
                '        iniFile.Close()
                '    End If
            End Try
        Else
            Throw New FileNotFoundException("Unable to locate " & iniPath)
        End If
    End Sub

    ''' <summary>
    ''' Returns the value for the given section, key pair.
    ''' </summary>
    ''' <param name="sectionName">Section name.</param>
    Public Function GetSetting(sectionName As String, settingName As String) As String
        Dim sectionPair As SectionPair
        sectionPair.Section = sectionName.ToUpper()
        sectionPair.Key = settingName.ToUpper()

        Return DirectCast(keyPairs(SectionPair), String)
    End Function

    'Public Function SectionExists(sectionName As String) As Boolean
    '    Return keyPairs.ContainsKey(sectionName.ToUpper)
    'End Function

    Public Function EnumSection(sectionName As String) As String()
        Dim tmpArray As New ArrayList()

        For Each pair As SectionPair In keyPairs.Keys
            If pair.Section = sectionName.ToUpper() Then
                tmpArray.Add(pair.Key)
            End If
        Next

        Return DirectCast(tmpArray.ToArray(GetType(String)), String())
    End Function

    ''' <summary>
    ''' Adds or replaces a setting to the table to be saved.
    ''' </summary>
    ''' <param name="sectionName">Section to add under.</param>
    ''' <param name="settingName">Key name to add.</param>
    ''' <param name="settingValue">Value of key.</param>
    Public Sub AddSetting(sectionName As String, settingName As String, settingValue As String)
        Dim sectionPair As SectionPair
        sectionPair.Section = sectionName.ToUpper()
        sectionPair.Key = settingName.ToUpper()

        If keyPairs.ContainsKey(sectionPair) Then
            keyPairs.Remove(sectionPair)
        End If

        keyPairs.Add(sectionPair, settingValue)
    End Sub

    ''' <summary>
    ''' Adds or replaces a setting to the table to be saved with a null value.
    ''' </summary>
    ''' <param name="sectionName">Section to add under.</param>
    ''' <param name="settingName">Key name to add.</param>
    Public Sub AddSetting(sectionName As String, settingName As String)
        AddSetting(sectionName, settingName, Nothing)
    End Sub

    ''' <summary>
    ''' Remove a setting.
    ''' </summary>
    ''' <param name="sectionName">Section to add under.</param>
    ''' <param name="settingName">Key name to add.</param>
    Public Sub DeleteSetting(sectionName As String, settingName As String)
        Dim sectionPair As SectionPair
        sectionPair.Section = sectionName.ToUpper()
        sectionPair.Key = settingName.ToUpper()

        If keyPairs.ContainsKey(sectionPair) Then
            keyPairs.Remove(sectionPair)
        End If
    End Sub

    ''' <summary>
    ''' Save settings to new file.
    ''' </summary>
    ''' <param name="newFilePath">New file path.</param>
    Public Sub SaveSettings(newFilePath As String)
        Dim sections As New ArrayList()
        Dim tmpValue As String = ""
        Dim strToSave As String = ""

        For Each sectionPair As SectionPair In keyPairs.Keys
            If Not sections.Contains(sectionPair.Section) Then
                sections.Add(sectionPair.Section)
            End If
        Next

        For Each section As String In sections
            strToSave += ("[" & section & "]" & Convert.ToChar(13) & Convert.ToChar(10))

            For Each sectionPair As SectionPair In keyPairs.Keys
                If sectionPair.Section = section Then
                    tmpValue = DirectCast(keyPairs(sectionPair), String)

                    If tmpValue IsNot Nothing Then
                        tmpValue = sectionPair.Key & "=" & tmpValue
                    End If

                    strToSave += (tmpValue & Convert.ToChar(13) & Convert.ToChar(10))
                End If
            Next

            strToSave += Convert.ToChar(13) & Convert.ToChar(10)
        Next

        Try
            Dim tw As TextWriter = New StreamWriter(newFilePath)
            tw.Write(strToSave)
            tw.Close()
        Catch ex As Exception
            Throw 'ex
        End Try
    End Sub

    ''' <summary>
    ''' Save settings back to ini file.
    ''' </summary>
    Public Sub SaveSettings()
        SaveSettings(iniFilePath)
    End Sub
End Class


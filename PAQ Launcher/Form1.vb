Imports System
Imports System.IO
Imports System.Net

Public Class Form1
#Region "dims"
    Dim webResponse = ""
    Dim CurrentVersion, UserName, sessionid As String
    Dim tempFiles As String = "C:\PAQ-Temp\"
    Dim curentLocation As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
    Dim WithEvents WC As New WebClient
    Declare Function GetUserName Lib "advapi32.dll" Alias "GetUserNameA" (ByVal lpBuffer As String, ByRef nSize As Integer) As Integer
    Dim pathPAQ As String
    Dim PAQv, PAQLauncherV As String
#End Region

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        System.Diagnostics.Process.Start("http://pack.mage-tech.org/")
    End Sub

    Private Sub Form1_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        Dim options As New IO.StreamWriter(pathPAQ + "\LOptions.txt")
        options.WriteLine(PAQv)
        options.WriteLine(PAQLauncherV)
        options.WriteLine(pathPAQ)
        options.WriteLine(txtUserName.Text)
        options.WriteLine(txtPassword.Text)
        options.Close()
        tempfiledelete()
    End Sub
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        curentLocation = curentLocation.Remove(0, 6)
        pathPAQ = curentLocation
        Dim webPAQv As String
        Dim webPAQLauncherV As String
        Dim value As Boolean
        webPAQv = WC.DownloadString("http://mage-tech.org/pack/PAQv.txt")
        webPAQLauncherV = WC.DownloadString("http://mage-tech.org/pack/PAQLauncherV.txt")
        If IO.File.Exists(curentLocation & "v.txt") = True Then
            Dim objReader As New System.IO.StreamReader(curentLocation & "v.txt")
                PAQv = objReader.ReadLine()
                PAQLauncherV = objReader.ReadLine()
                pathPAQ = objReader.ReadLine()
                txtUserName.Text = objReader.ReadLine()
                txtPassword.Text = objReader.ReadLine()
            objReader.Dispose()
            value = True
        Else
            PAQv = webPAQv
            PAQLauncherV = webPAQLauncherV
            value = False
        End If
        If value = True Then
            If PAQv = webPAQv And PAQLauncherV = webPAQLauncherV Then
                btnDL.Visible = False
            End If
        End If
        tempfilecrate()
    End Sub
#Region "Launch"
    Private Sub btnLaunch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLaunch.Click
        startminecraft(txtUserName.Text, txtPassword.Text, "C:\The PACK\test server\")
    End Sub
    Sub startminecraft(ByRef user As String, ByRef password As String, ByRef batloc As String)
        Dim webstream As Stream
        Dim req As HttpWebRequest
        Dim res As HttpWebResponse
        Dim Session(5) As String
        Dim loc1 As Short = 0
        Dim loc2 As Short

        req = WebRequest.Create("https://login.minecraft.net?user=" & user & "&password=" & password & "&version=13")
        req.Method = "GET"
        res = req.GetResponse()
        webstream = res.GetResponseStream()
        Dim webSteamReader As New StreamReader(webstream)
        While webSteamReader.Peek >= 0
            webResponse = webSteamReader.ReadToEnd()
        End While

        If IO.File.Exists(pathPAQ + "\l.bat") = False Then
            Try
                WC.DownloadFile(New Uri("http://mage-tech.org/pack/l.bat"), pathPAQ & "\l.bat")
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try

        End If

        If webResponse.ToString = Nothing Then
            MsgBox("Error")
        ElseIf webResponse.ToString = "Bad login" Then
            MsgBox("Bad login please check login info")
        ElseIf webResponse.ToString = "Account migrated, use e-mail as username." Then
            MsgBox("Account migrated, use e-mail as username.")
        Else
            Dim wr As String = webResponse.ToString
            For i = 1 To 5
                loc2 = wr.IndexOf(":", loc1 + 1)
                If loc2 = -1 Then
                    loc2 = wr.Length
                End If
                Session(i) = wr.Substring(loc1, (loc2 - loc1))
                loc1 = wr.IndexOf(":", loc1 + 1)
            Next
            CurrentVersion = Session(1)
            UserName = Session(3).Remove(0, 1)
            sessionid = Session(4).Remove(0, 1)
            Dim arg As String = "512m" & " " & "1024m" & " " & "1024m" & " " & UserName & " " & sessionid
            MsgBox(arg)
            Process.Start(batloc & "l.bat", arg)
        End If
    End Sub
#End Region
#Region "instal"
    Private Sub btnDL_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDL.Click
        PAQCreate()
    End Sub
    Sub download()
        Dim downloadlist As String = tempFiles & "PAQ.txt"
        'Dim zipTF As Boolean
        'Dim saveloc As String
        If IO.File.Exists(tempFiles & "PAQ.txt") = False Then
            WC.DownloadFile(New Uri("http://mage-tech.org/pack/PAQ.txt"), tempFiles & "PAQ.txt")
        Else
            IO.File.Delete(tempFiles & "PAQ.txt")
            WC.DownloadFile(New Uri("http://mage-tech.org/pack/PAQ.txt"), tempFiles & "PAQ.txt")
        End If
    End Sub
#End Region
#Region "create / delete files"
    Sub tempfilecrate()
        Try
            If IO.Directory.Exists("C:\PAQ-Temp") = False Then
                IO.Directory.CreateDirectory("C:\PAQ-Temp")
                Select Case IO.Directory.Exists(tempFiles & "mods") Or IO.Directory.Exists(tempFiles & "config") _
                    Or IO.Directory.Exists(tempFiles & "coremods") Or IO.Directory.Exists(tempFiles & "toJar")
                    Case Is = False
                        IO.Directory.CreateDirectory(tempFiles & "mods")
                        IO.Directory.CreateDirectory(tempFiles & "config")
                        IO.Directory.CreateDirectory(tempFiles & "coremods")
                        IO.Directory.CreateDirectory(tempFiles & "toJar")
                End Select
            End If
            If IO.Directory.Exists(curentLocation.ToString & "Downloads") = False Then
                IO.Directory.CreateDirectory(curentLocation.ToString & "Downloads")
            End If
        Catch ex As Exception
            MsgBox("error in makeing TempDirectorys: " & ex.ToString)
        End Try
    End Sub
    Sub tempfiledelete()
        If IO.Directory.Exists("C:\DNS-Temp") Then
            Try
                IO.Directory.Delete("C:\DNS-Temp", True)
            Catch ex As Exception
                MsgBox("Exception: " & ex.ToString)
            End Try
        End If
    End Sub
    Sub PAQCreate()
        Dim pathMc As String
        MsgBox("please select the .minecraft location or hit cancel to use default")
        fbdPathMc.SelectedPath = "C:\Users\" + GetUserName() + "\AppData\Roaming\.minecraft"
        If fbdPathMc.ShowDialog() = Windows.Forms.DialogResult.OK Then
            pathMc = fbdPathMc.SelectedPath()
        Else
            If IO.Directory.Exists("C:\Users\" + GetUserName() + "\AppData\Roaming\.minecraft") = False Then
                MsgBox(".minecraft does not exist please launch minecraft at least once with the normal launcher")
            Else
                pathMc = "C:\Users\" + GetUserName() + "\AppData\Roaming\.minecraft"
            End If
        End If
        MsgBox("please select the PAQ install location or hit cancel to use default")
        If fbdPathPAQ.ShowDialog() = Windows.Forms.DialogResult.OK Then
            pathPAQ = fbdPathPAQ.SelectedPath()
        Else
            If IO.Directory.Exists("C:\Users\" + GetUserName() + "\AppData\Roaming\.PAQ") = False Then
                IO.Directory.CreateDirectory("C:\Users\" + GetUserName() + "\AppData\Roaming\.PAQ")
            End If
            pathPAQ = "C:\Users\" + GetUserName() + "\AppData\Roaming\.PAQ"
        End If

        If IO.Directory.Exists(pathMc) Then
            My.Computer.FileSystem.CopyDirectory(pathMc, pathPAQ, True)
            MsgBox("copy done")
        End If
    End Sub
    Public Function GetUserName() As String
        Dim iReturn As Integer
        Dim userName As String
        userName = New String(CChar(" "), 50)
        iReturn = GetUserName(userName, 50)
        GetUserName = userName.Substring(0, userName.IndexOf(Chr(0)))
    End Function
#End Region


End Class

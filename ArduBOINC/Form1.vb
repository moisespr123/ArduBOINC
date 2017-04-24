Imports System.IO.Ports
Imports BoincRpc

Public Class Form1
    Dim ArduinoSerialPort As New SerialPort
    Dim COMSetupComplete As Boolean = False
    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If COMSetupComplete = False Then
            Try
                With ArduinoSerialPort
                    .PortName = "COM" & NumericUpDown2.Value
                    .BaudRate = 38400
                    .Parity = Parity.None
                    .DataBits = 8
                    .StopBits = 1
                    .DtrEnable = True
                    .ReadTimeout = 30000
                    .Open()
                End With
                COMSetupComplete = True
            Catch ex As Exception
                MsgBox("Another program is already using COM" & NumericUpDown2.Value & "." & vbCrLf & vbCrLf &
                      "Please try again later", MsgBoxStyle.OkOnly + MsgBoxStyle.Information, "COM" & NumericUpDown2.Value & " Not Available")
            End Try
            BackgroundWorker1.RunWorkerAsync()
        End If
    End Sub
    Private Sub SendTaskDetails(wuname As String, project As String, percent As String, remainingtime As TimeSpan)
        Dim TimeFormated As String = String.Format("{0}:{1:mm}:{1:ss}", CInt(Math.Truncate(remainingtime.TotalHours)), remainingtime)
        Threading.Thread.Sleep(1000)
        If ArduinoSerialPort.IsOpen = False Then ArduinoSerialPort.Open()
        ArduinoSerialPort.Write(wuname & "|" & project & "|" & percent & "|ETA: " & TimeFormated)
        Dim ReceivedData = 0
        Dim ReceivedLine As String = ""
        While ReceivedData = 0
            ReceivedLine = ArduinoSerialPort.ReadLine()
            If ReceivedLine = "OK" & vbCr Then ReceivedData = 1
        End While
    End Sub

    Private Async Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim BOINCClient As New RpcClient
        Await BOINCClient.ConnectAsync(TextBox1.Text, TextBox2.Text)
        Dim Authorized As Boolean = Await BOINCClient.AuthorizeAsync(TextBox3.Text)
        If Authorized Then
            While (True)
                Threading.Thread.Sleep(1000)
                For Each result In Await BOINCClient.GetResultsAsync()
                    If result.ActiveTask = True Then
                        Dim Project As String = ""
                        If result.ProjectUrl = "http://www.worldcommunitygrid.org/" Or result.ProjectUrl = "https://www.worldcommunitygrid.org/" Then
                            Project = "World Community Grid"
                        ElseIf result.ProjectUrl = "https://boinc.thesonntags.com/collatz/" Or result.ProjectUrl = "http://boinc.thesonntags.com/collatz/" Then
                            Project = "Collatz Conjecture"
                        ElseIf result.ProjectUrl = "https://wuprop.boinc-af.org/" Or result.ProjectUrl = "http://wuprop.boinc-af.org/" Then
                            Project = "WUProp@Home"
                        ElseIf result.ProjectUrl = "https://moowrap.net/" Or result.ProjectUrl = "http://moowrap.net/" Then
                            Project = "Moo! Wrapper"
                        ElseIf result.ProjectUrl = "http://www.bitcoinutopia.net/bitcoinutopia/" Or result.ProjectUrl = "http://bitcoinutopia.net/bitcoinutopia/" Then
                            Project = "Bitcoin Utopia"
                        ElseIf result.ProjectUrl = "http://setiathome.berkeley.edu/" Then
                            Project = "SETI@Home"
                        ElseIf result.ProjectUrl = "http://asteroidsathome.net/boinc/" Then
                            Project = "Asteroids@Home"
                        ElseIf result.ProjectUrl = "http://goofyxgridathome.net/" Then
                            Project = "GoofyxGrid@Home"
                        ElseIf result.ProjectUrl = "http://cpu.goofyxgridathome.net/" Then
                            Project = "GoofyxGrid@Home CPU"
                        ElseIf result.ProjectUrl = "http://finance.gridcoin.us/finance/" Then
                            Project = "Gridcoin Finance"
                        Else
                            Project = result.ProjectUrl
                        End If
                        SendTaskDetails(result.WorkunitName, Project, result.FractionDone * 100 & "%", result.EstimatedCpuTimeRemaining)
                    End If
                Next
            End While

        End If
    End Sub
End Class

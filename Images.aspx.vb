Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Drawing
Imports AjaxControlToolkit

'Reffing this article: http://www.4guysfromrolla.com/articles/120606-1.aspx and BLOBsInDB code in /Mine for 
'managing images stored as binary data

Partial Class Images
    Inherits System.Web.UI.Page

    Dim oConn As New SqlConnection(ConfigurationManager.ConnectionStrings("GiftListConnectionString").ConnectionString)
    Dim oCmd As New SqlCommand
    Dim oParam As SqlParameter
    Dim oDA As New SqlDataAdapter
    Dim oDTbl As New DataTable
    Dim strSQL As String = ""

    Dim msgScript As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then
            If Session("SelectedImagesGift") IsNot Nothing Then

                'Automatically go to insert mode if coming from an "Add Images" link (no images exist)
                If imagesForGift(Session("SelectedImagesGift")) = 0 Then
                    images_fv.ChangeMode(FormViewMode.Insert)
                    configElements(False)
                    bindImagesDDL(Session("SelectedImagesGift"), -1)
                Else
                    configElements(True)
                    bindImagesDDL(Session("SelectedImagesGift"))
                End If

                gift_lbl.DataBind()
                prompt_lit.Text = "<span class='PromptStyle'>Select an image to edit, or add a new one.</span>"

            Else
                Response.Redirect("Default.aspx")
            End If

        End If

    End Sub

    Private Sub bindImagesDDL(ByVal giftID As Integer, Optional ByVal value As Integer = -2)
        Dim tempStrSQL As String = ""

        Try
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            tempStrSQL = "SELECT ID, ImageTitle FROM Images WHERE GiftID = @GiftID UNION SELECT -2 AS ID, '--Select Image--' AS ImageTitle UNION SELECT -1 AS ID, 'ADD NEW' AS ImageTitle ORDER BY ID"

            oCmd.Parameters.Clear()
            oDTbl.Clear()

            oParam = New SqlParameter
            oParam.ParameterName = "GiftID"
            oParam.Value = giftID
            oParam.SqlDbType = SqlDbType.Int
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = tempStrSQL
            oDA.SelectCommand = oCmd

            oDA.Fill(oDTbl)

            images_ddl.DataSource = oDTbl
            images_ddl.DataBind()

            images_ddl.SelectedValue = value

            viewAll_lBtn.Visible = (imagesForGift(Session("SelectedImagesGift")) > 0)

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub configElements(ByVal showPrompt As Boolean)
        prompt_lit.Visible = showPrompt
        outerCancel_btn.Visible = showPrompt
        images_fv.Visible = Not showPrompt
    End Sub

    Private Sub bindImagesDList(ByVal giftID As Integer)
        Dim tempStrSQL As String = ""

        Try
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            tempStrSQL = "SELECT * FROM Images WHERE GiftID = @GiftID"

            oCmd.Parameters.Clear()
            oDTbl.Clear()

            oParam = New SqlParameter
            oParam.ParameterName = "GiftID"
            oParam.Value = giftID
            oParam.SqlDbType = SqlDbType.Int
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = tempStrSQL
            oDA.SelectCommand = oCmd

            oDA.Fill(oDTbl)

            images_dList.DataSource = oDTbl
            images_dList.DataBind()

        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Private Function imagesForGift(ByVal giftID As Integer) As Integer

        Try
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            strSQL = "SELECT Count(*) FROM Images WHERE GiftID = @ID"

            oCmd.Parameters.Clear()

            oParam = New SqlParameter()
            oParam.ParameterName = "ID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = giftID
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = strSQL
            oCmd.Connection.Open()

            Return oCmd.ExecuteScalar()
        Catch ex As Exception
            Throw ex
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If

            oCmd.Dispose()
        End Try

    End Function


    Protected Sub imagesSelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Select Case images_ddl.SelectedValue
            Case -1
                images_fv.ChangeMode(FormViewMode.Insert)
            Case -2
                Session("SelectedImage") = Nothing
            Case Else
                Session("SelectedImage") = images_ddl.SelectedValue
                images_fv.ChangeMode(FormViewMode.ReadOnly)
        End Select
        images_fv.Visible = (images_ddl.SelectedValue <> -2)
        prompt_lit.Visible = (images_ddl.SelectedValue = -2)
        outerCancel_btn.Visible = (images_ddl.SelectedValue = -2)
        viewImages_pnl.Visible = False

    End Sub

    Protected Sub viewAllClick(ByVal sender As Object, ByVal e As System.EventArgs)
        images_ddl.SelectedValue = -2
        configElements(True)
        bindImagesDList(Session("SelectedImagesGift"))
        viewImages_pnl.Visible = True
    End Sub

    Public Function formatGiftHeader() As String

        Dim gift As String = ""
        Dim recipient As String = ""

        Try
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            strSQL = "SELECT Description FROM Gifts WHERE ID = @ID"

            oCmd.Parameters.Clear()

            oParam = New SqlParameter
            oParam.ParameterName = "ID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = Session("SelectedImagesGift")
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = strSQL

            oCmd.Connection.Open()
            gift = oCmd.ExecuteScalar()

            strSQL = "SELECT FirstName, LastName FROM Recipients WHERE ID = @ID"

            oCmd.Parameters.Clear()
            oDTbl.Clear()

            oParam = New SqlParameter
            oParam.ParameterName = "ID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = Session("SelectedGiftRecipient")
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = strSQL
            oDA.SelectCommand = oCmd
            oDA.Fill(oDTbl)

            Return "<h3>Add/Edit Images for " & gift & ", gift for " & oDTbl.Rows(0)("FirstName") & IIf(oDTbl.Rows(0)("LastName") IsNot DBNull.Value, " " & oDTbl.Rows(0)("LastName"), "") & "</h3>"
        Catch ex As Exception
            Throw ex
        Finally

            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If

            oCmd.Dispose()
        End Try
    End Function

    Public Function formatImageStats() As String

        Dim imageBytes() As Byte = getBinaryImage(Session("SelectedImage"))
        Dim img As Image = getImageFromBytes(imageBytes)

        Return "Size in database: <b>" & IIf(imageBytes.Length >= 1048576, Math.Round(imageBytes.Length) & "MB", Math.Round(imageBytes.Length / 1024, 2) & "KB") & "</b><br />Dimensions: <b>" & img.Width & " x " & img.Height & "</b>"
    End Function


    Public Function formatToolTipStats(ByVal id As Object, ByVal title As Object) As String
        Dim imagebytes() As Byte = getBinaryImage(id)
        Dim img As Image = getImageFromBytes(imagebytes)

        Return "<b>" & title & "</b><br />Size in database: <b>" & IIf(imagebytes.Length >= 1048576, Math.Round(imagebytes.Length) & "MB", Math.Round(imagebytes.Length / 1024, 2) & "KB") & "</b><br />Dimensions: <b>" & img.Width & " x " & img.Height & "</b>"
    End Function

    Protected Sub images_fv_ItemCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewCommandEventArgs) Handles images_fv.ItemCommand
        If e.CommandName = "Cancel" Then
            Response.Redirect("Default.aspx")
        End If
    End Sub

    Protected Sub images_fv_ItemInserting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewInsertEventArgs) Handles images_fv.ItemInserting
        Dim uploader As FileUpload = CType(images_fv.FindControl("image_uploader"), FileUpload)
        Dim MIMEType As String = ""

        Try
            If uploader.HasFile Then
                Dim ext As String = Path.GetExtension(uploader.PostedFile.FileName).ToLower()

                Select Case ext
                    Case ".gif"
                        MIMEType = "image/gif"
                    Case ".jpg"
                        MIMEType = "image/jpeg"
                    Case ".png"
                        MIMEType = "image/png"
                    Case Else
                        Throw New ArgumentException("Image must be in either GIF, JPG or PNG format.")
                End Select

                Dim imageBytes(uploader.PostedFile.InputStream.Length) As Byte
                uploader.PostedFile.InputStream.Read(imageBytes, 0, imageBytes.Length)

                e.Values("ImageData") = imageBytes
                e.Values("MIMEType") = MIMEType
            Else
                Throw New ArgumentException("Please select an image to upload.")
            End If
        Catch ex As ArgumentException
            msgScript = "<script language='javascript'>" & _
                       "alert('Error inserting record:\n\n" & ex.Message & "');" & _
                       "</script>"
            Page.ClientScript.RegisterStartupScript(Me.GetType(), "msgScript", msgScript)
            e.Cancel = True

        Catch ex As Exception
            msgScript = "<script language='javascript'>" & _
                        "alert('Uh oh. Error updating record.');" & _
                        "</script>"

            Page.ClientScript.RegisterStartupScript(Me.GetType(), "msgScript", msgScript)
            e.Cancel = True

        End Try
    End Sub

    Protected Sub images_fv_ItemUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewUpdateEventArgs) Handles images_fv.ItemUpdating
        Dim uploader As FileUpload = CType(images_fv.FindControl("image_uploader"), FileUpload)
        Dim MIMEType As String = ""

        Try
            If uploader.HasFile Then

                Dim ext As String = Path.GetExtension(uploader.PostedFile.FileName).ToLower()

                Select Case ext
                    Case ".gif"
                        MIMEType = "image/gif"
                    Case ".jpg"
                        MIMEType = "image/jpeg"
                    Case ".png"
                        MIMEType = "image/png"
                    Case Else
                        Throw New ArgumentException("Image must be in either GIF, JPG or PNG format.")
                End Select

                Dim imageBytes(uploader.PostedFile.InputStream.Length) As Byte
                uploader.PostedFile.InputStream.Read(imageBytes, 0, imageBytes.Length)

                e.NewValues("ImageData") = imageBytes
                e.NewValues("MIMEType") = MIMEType
            Else
                e.NewValues("ImageData") = getBinaryImage(Session("SelectedImage"))
                e.NewValues("MIMEType") = e.OldValues("MIMEType")
            End If

        Catch ex As ArgumentException
            msgScript = "<script language='javascript'>" & _
                                           "alert('Error updating record:\n\n" & ex.Message & "');" & _
                                           "</script>"
            Page.ClientScript.RegisterStartupScript(Me.GetType(), "msgScript", msgScript)
            e.Cancel = True

        Catch ex As Exception
            msgScript = "<script language='javascript'>" & _
                        "alert('Uh oh. Error updating record.');" & _
                        "</script>"

            Page.ClientScript.RegisterStartupScript(Me.GetType(), "msgScript", msgScript)
            e.Cancel = True
        End Try

    End Sub

    Protected Sub images_fv_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewInsertedEventArgs) Handles images_fv.ItemInserted
        images_fv.ChangeMode(FormViewMode.ReadOnly)
        bindImagesDDL(Session("SelectedImagesGift"), Session("SelectedImage"))
    End Sub

    Protected Sub images_fv_ItemUpdated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewUpdatedEventArgs) Handles images_fv.ItemUpdated
        images_fv.ChangeMode(FormViewMode.ReadOnly)
        bindImagesDDL(Session("SelectedImagesGift"), Session("SelectedImage"))
    End Sub


    Protected Sub images_fv_ItemDeleted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewDeletedEventArgs) Handles images_fv.ItemDeleted
        configElements(True)
        bindImagesDDL(Session("SelectedImagesGift"))

    End Sub

    Protected Sub images_fv_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles images_fv.DataBound
        setPostbackTriggers()

        If images_fv.CurrentMode <> FormViewMode.Insert Then
            Dim pnl As Panel = CType(images_fv.FindControl("previewImg_pnl"), Panel)
            Dim newDims As Size = getResizedDimensions(getImageFromBytes(getBinaryImage(Session("SelectedImage"))), 400)

            Dim imgStr As String = "<a href='DisplayImage.aspx?ID=" & Session("SelectedImage") & "' target='_blank'><img src='DisplayImage.aspx?ID=" & Session("SelectedImage") & "' class='ImageStyle' width='" & newDims.Width & "' height='" & newDims.Height & "' /></a>"

            pnl.Controls.Add(New LiteralControl(imgStr))
        End If
    End Sub


    Protected Sub image_sds_Inserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.SqlDataSourceStatusEventArgs) Handles image_sds.Inserted
        Session("SelectedImage") = CType(e.Command.Parameters("@NewID").Value, Integer)
    End Sub

    'This byte can be saved to the e.values of a varbinary type when updating
    Private Function getBinaryImage(ByVal id As Integer) As Byte()

        Try
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            strSQL = "SELECT ImageData FROM Images WHERE ID = @ID"

            oCmd.Parameters.Clear()

            oParam = New SqlParameter
            oParam.ParameterName = "ID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = id
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = strSQL

            oCmd.Connection.Open()
            Dim reader As SqlDataReader = oCmd.ExecuteReader()
            reader.Read()

            Return CType(reader("ImageData"), Byte())

        Catch ex As Exception
            Throw ex
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If

            oCmd.Dispose()
        End Try

    End Function

    Private Function getImageFromBytes(ByVal bytes As Byte()) As Image
        Dim ms As MemoryStream = New MemoryStream(bytes, 0, bytes.Length)
        ms.Write(bytes, 0, bytes.Length)

        Return Image.FromStream(ms)

    End Function

    Private Function getResizedDimensions(ByVal img As Image, ByVal maxDim As Integer) As Size
        Dim newDims As Size

        'Originally set to initial dimensions so that if both dimensions don't meet any of the criteria, the sizes will remain as is
        newDims.Height = img.Height
        newDims.Width = img.Width

        If (img.Height > maxDim And img.Width < maxDim) Or (img.Height > maxDim And img.Width > maxDim And img.Height > img.Width) Then
            newDims.Height = maxDim
            newDims.Width = img.Width / img.Height * newDims.Height
        ElseIf (img.Width > maxDim And img.Height < maxDim) Or (img.Height > maxDim And img.Width > maxDim And img.Width > img.Height) Then
            newDims.Width = maxDim
            newDims.Height = img.Height / img.Width * newDims.Width
        ElseIf (img.Width > maxDim And img.Height > maxDim And img.Width = img.Height) Then
            newDims.Height = maxDim
            newDims.Width = maxDim
        End If

        Return newDims
    End Function

    Protected Sub outerCancelClick(ByVal sender As Object, ByVal e As System.EventArgs)
        Response.Redirect("Default.aspx")
    End Sub

    Private Sub setPostbackTriggers()
        Dim insert_btn As Button
        Dim update_btn As Button
        If images_fv.CurrentMode = FormViewMode.Edit Then
            update_btn = CType(images_fv.FindControl("update_btn"), Button)
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(update_btn)
        End If

        If images_fv.CurrentMode = FormViewMode.Insert Then
            insert_btn = CType(images_fv.FindControl("insert_btn"), Button)
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(insert_btn)
        End If
    End Sub

    Protected Sub images_dList_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs) Handles images_dList.ItemDataBound
        Dim pnl As Panel = CType(e.Item.FindControl("listImg_pnl"), Panel)
        Dim newDims As Size = getResizedDimensions(getImageFromBytes(getBinaryImage(e.Item.DataItem("ID"))), 138)

        Dim imgStr As String = "<a href='DisplayImage.aspx?ID=" & e.Item.DataItem("ID") & "' target='_blank'><img src='DisplayImage.aspx?ID=" & e.Item.DataItem("ID") & "' class='ImageStyle' width='" & newDims.Width & "' height='" & newDims.Height & "' /></a>"

        pnl.Controls.Add(New LiteralControl(imgStr))

    End Sub

End Class

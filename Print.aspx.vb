Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Imports System.IO

Partial Class Print
    Inherits System.Web.UI.Page

    Dim oConn As New SqlConnection(ConfigurationManager.ConnectionStrings("GiftListConnectionString").ConnectionString)
    Dim oCmd As New SqlCommand
    Dim oParam As SqlParameter
    Dim oDA As New SqlDataAdapter
    Dim oDTbl As New DataTable
    Dim inner_dTbl As New DataTable
    Dim gifts_dTbl As New DataTable
    Dim imgs_dTbl As New DataTable
    Dim strSQL As String = ""


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim now As DateTime = DateTime.Now.Date.ToShortDateString
        If Not IsPostBack Then
            bindCategories()

            title_lit.Text = "<h3>" & IIf(Session("ListTitle") IsNot Nothing, Session("ListTitle"), "Gift List") & " as of " & now & "</h3>"
            summary_lit.Text = summaryString()
        End If

    End Sub

    Private Function summaryString() As String

        Dim numCategories As Integer
        Dim numRecipients As Integer
        Dim numGifts As Integer

        Try
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            oCmd.Parameters.Clear()

            strSQL = "SELECT Count(*) FROM Categories"
            oCmd.CommandText = strSQL

            oCmd.Connection.Open()

            numCategories = oCmd.ExecuteScalar()

            strSQL = "SELECT Count(*) FROM Recipients"
            oCmd.CommandText = strSQL

            numRecipients = oCmd.ExecuteScalar()

            strSQL = "SELECT Count(*) FROM Gifts"
            oCmd.CommandText = strSQL

            numGifts = oCmd.ExecuteScalar()

            Return "<span class='Divider'>&nbsp;</span><br /><div class='SummaryStyle'>Categories: <b>" & IIf(numCategories > 0, numCategories, "<i>None</i>") & "</b><br />Recipients: <b>" & IIf(numRecipients > 0, numRecipients, "<i>None</i>") & "</b><br />Gifts: <b>" & IIf(numGifts > 0, numGifts, "<i>None</i>") & "</b></div>"

        Catch ex As Exception
            Throw ex
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If

            oCmd.Dispose()
        End Try

    End Function

    Protected Sub bindCategories()

        'Binding the child controls using the parent controls' datatables ensures they'll exist when I call findControl
        Try
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            strSQL = "SELECT * FROM Categories ORDER BY Title"

            oCmd.Parameters.Clear()
            oDTbl.Clear()

            oCmd.CommandText = strSQL
            oDA.SelectCommand = oCmd

            oDA.Fill(oDTbl)

            cat_repeater.DataSource = oDTbl
            cat_repeater.DataBind()

            For i As Integer = 0 To oDTbl.Rows.Count - 1

                oCmd.Parameters.Clear()
                inner_dTbl.Clear()

                strSQL = "SELECT *, Title FROM Recipients INNER JOIN Categories on Recipients.CategoryID = Categories.ID WHERE CategoryID = @CategoryID ORDER BY LastName"

                oParam = New SqlParameter
                oParam.ParameterName = "CategoryID"
                oParam.SqlDbType = SqlDbType.Int
                oParam.Value = oDTbl.Rows(i)("ID")
                oCmd.Parameters.Add(oParam)

                oCmd.CommandText = strSQL
                oDA.SelectCommand = oCmd

                oDA.Fill(inner_dTbl)

                Dim gv As GridView = CType(cat_repeater.Items(i).FindControl("recipients_gv"), GridView)
                gv.DataSource = inner_dTbl
                gv.DataBind()


                For j As Integer = 0 To inner_dTbl.Rows.Count - 1

                    oCmd.Parameters.Clear()
                    gifts_dTbl.Clear()

                    strSQL = "SELECT * FROM Gifts WHERE RecipientID = @RecipientID ORDER BY Description"

                    oParam = New SqlParameter
                    oParam.ParameterName = "RecipientID"
                    oParam.SqlDbType = SqlDbType.Int
                    oParam.Value = inner_dTbl.Rows(j)("ID")
                    oCmd.Parameters.Add(oParam)

                    oCmd.CommandText = strSQL
                    oDA.SelectCommand = oCmd

                    oDA.Fill(gifts_dTbl)

                    Dim inner_gv As GridView = CType(gv.Rows(j).FindControl("gifts_gv"), GridView)
                    inner_gv.DataSource = gifts_dTbl
                    inner_gv.DataBind()

                    If Session("PrintImages") Then
                        For k As Integer = 0 To gifts_dTbl.Rows.Count - 1

                            oCmd.Parameters.Clear()
                            imgs_dTbl.Clear()

                            strSQL = "SELECT * FROM Images WHERE GiftID = @GiftID"

                            oParam = New SqlParameter
                            oParam.ParameterName = "GiftID"
                            oParam.SqlDbType = SqlDbType.Int
                            oParam.Value = gifts_dTbl.Rows(k)("ID")
                            oCmd.Parameters.Add(oParam)

                            oCmd.CommandText = strSQL
                            oDA.SelectCommand = oCmd

                            oDA.Fill(imgs_dTbl)

                            Dim dList As DataList = CType(inner_gv.Rows(k).FindControl("images_dList"), DataList)
                            'So that the DataList header won't show if no images exist
                            dList.Visible = (imgs_dTbl.Rows.Count > 0)
                            dList.DataSource = imgs_dTbl
                            dList.DataBind()

                        Next
                    End If


                    Dim cat_img As System.Web.UI.WebControls.Image = CType(cat_repeater.Items(i).FindControl("catComplete_img"), System.Web.UI.WebControls.Image)
                    Dim catID As Integer = CType(cat_repeater.Items(i).FindControl("id_hField"), HiddenField).Value
                    Dim catRowStatus As String = getCategoryCompleteStatus(catID)

                    If Session("PrintMode") = "Checklist" Then
                        cat_img.ImageUrl = "checkboxBlankCategory.png"
                    Else
                        Select Case catRowStatus
                            Case "Complete"
                                cat_img.ImageUrl = "checkmarkCategory.png"
                            Case "GiftsExist"
                                cat_img.ImageUrl = "checkboxBlankCategory.png"
                        End Select
                    End If

                    cat_img.Visible = (catRowStatus <> "NoGifts")
                Next

            Next

        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Protected Sub giftsRowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)

        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim gift_img As System.Web.UI.WebControls.Image = CType(e.Row.FindControl("giftComplete_img"), System.Web.UI.WebControls.Image)
            gift_img.ImageUrl = IIf(Session("PrintMode") = "Checklist", "checkboxBlankGift.png", IIf(e.Row.DataItem("Complete") = True, "checkmarkGift.png", "checkboxBlankGift.png"))
        End If


    End Sub


    Protected Sub recipientsRowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)

        Dim rowStatus As String

        If e.Row.RowType = DataControlRowType.DataRow Then

            Dim recip_img As System.Web.UI.WebControls.Image = CType(e.Row.FindControl("recipientComplete_img"), System.Web.UI.WebControls.Image)
            rowStatus = getRecipientCompleteStatus(CType(sender, GridView).DataKeys(e.Row.RowIndex).Values("ID"))

            recip_img.ImageUrl = IIf(Session("PrintMode") = "Checklist", "checkboxBlankRecipient.png", IIf(rowStatus = "Complete", "checkmarkRecipient.png", "checkboxBlankRecipient.png"))
        End If

    End Sub

    Protected Sub imagesItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs)

        'LEFT OFF HERE: Debug this functionality
        If e.Item.ItemType <> ListItemType.Header Then
            Dim pnl As Panel = CType(e.Item.FindControl("image_pnl"), Panel)
            Dim newDims As Size = getResizedDimensions(getImageFromBytesFromID(e.Item.DataItem("ID")), 200)

            Dim imgStr As String = "<img src='DisplayImage.aspx?ID=" & e.Item.DataItem("ID") & "' class='GVInnerImageStyle' width='" & newDims.Width & "' height='" & newDims.Height & "' />"
            pnl.Controls.Add(New LiteralControl(imgStr))
        End If


    End Sub

    Private Function getImageFromBytesFromID(ByVal id As Integer) As Image
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

            Dim bytes() As Byte = CType(reader("ImageData"), Byte())
            Dim ms As MemoryStream = New MemoryStream(bytes, 0, bytes.Length)

            ms.Write(bytes, 0, bytes.Length)

            Return Image.FromStream(ms)

        Catch ex As Exception
            Throw ex
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If

            oCmd.Dispose()
        End Try

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

    Private Function getCategoryCompleteStatus(ByVal catID As Integer) As String
        Dim numGiftsForCategory As Integer
        Dim numCompleteGiftsForCategory As Integer
        Dim status As String = ""

        Try

            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            'Get all gifts for category
            strSQL = "SELECT COUNT(*) FROM (SELECT Gifts.ID AS GiftID FROM Recipients INNER JOIN Gifts ON Recipients.ID = Gifts.RecipientID WHERE Recipients.CategoryID = @CategoryID) AS CategoryGifts"

            oCmd.Parameters.Clear()

            oParam = New SqlParameter()
            oParam.ParameterName = "CategoryID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = catID
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = strSQL

            oCmd.Connection.Open()
            numGiftsForCategory = oCmd.ExecuteScalar()

            'Get complete gifts for category
            strSQL = "SELECT COUNT(*) FROM (SELECT Gifts.ID AS GiftID FROM Recipients INNER JOIN Gifts ON Recipients.ID = Gifts.RecipientID WHERE Recipients.CategoryID = @CategoryID AND Gifts.Complete = 1) AS CategoryGifts"

            oCmd.Parameters.Clear()

            oParam = New SqlParameter()
            oParam.ParameterName = "CategoryID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = catID
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = strSQL

            numCompleteGiftsForCategory = oCmd.ExecuteScalar()

            If (numCompleteGiftsForCategory = numGiftsForCategory And numGiftsForCategory <> 0) Then
                status = "Complete"
            ElseIf numGiftsForCategory <> 0 Then
                status = "GiftsExist"
            ElseIf numGiftsForCategory = 0 Then
                status = "NoGifts"
            End If

            Return status

        Catch ex As Exception
            Throw ex
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If

            oCmd.Dispose()
        End Try

    End Function

    Private Function getRecipientCompleteStatus(ByVal recipID As Integer) As String

        Dim numTotalGifts As Integer
        Dim numCompleteGifts As Integer
        Dim status As String = ""

        Try
            'Select total gifts
            numTotalGifts = giftsForRecipient(recipID)

            'Select completed gifts
            strSQL = "SELECT COUNT(*) FROM Gifts WHERE RecipientID = @RecipientID AND Complete = 1"

            oParam = New SqlParameter()
            oParam.ParameterName = "RecipientID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = recipID
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = strSQL
            oCmd.Connection.Open()

            numCompleteGifts = oCmd.ExecuteScalar()

            If numTotalGifts = numCompleteGifts And numTotalGifts <> 0 Then
                status = "Complete"
            ElseIf numCompleteGifts < numTotalGifts And numCompleteGifts <> 0 Then
                status = "Partial"
            ElseIf numCompleteGifts = 0 Then
                status = "Empty"
            End If

            Return status

        Catch ex As Exception
            Throw ex
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If

            oCmd.Dispose()
        End Try

    End Function


    Private Function giftsForRecipient(ByVal recipID As Integer) As Integer

        Try
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            strSQL = "SELECT Count(*) FROM Gifts WHERE RecipientID = @ID"

            oCmd.Parameters.Clear()

            oParam = New SqlParameter()
            oParam.ParameterName = "ID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = recipID
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

    Private Function recipientsForCategory(ByVal catID As Integer) As Integer
        Try
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            strSQL = "SELECT Count(*) FROM Recipients WHERE CategoryID = @ID"

            oCmd.Parameters.Clear()

            oParam = New SqlParameter()
            oParam.ParameterName = "ID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = catID
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

    Public Function formatNameText(ByVal fn As Object, ByVal ln As Object) As String
        Return IIf(ln IsNot DBNull.Value, ln & ", ", "") & fn
    End Function

    Public Function configCatText(ByVal title As Object, ByVal catID As Integer) As String
        Dim numRecips As Integer = recipientsForCategory(catID)
        Return "<span class='CPanelText'>" & IIf(numRecips = 0, "<i>" & title & " [empty]</i>", title & ":" & IIf(numRecips > 1, " Recipients", " Recipient")) & "</span>"
    End Function


    Public Function configRecipText(ByVal recipID As Object) As String

        Dim numGifts = giftsForRecipient(recipID)
        Return IIf(numGifts = 0, "<div class='InnerCPanelStyle InnerEmptyText'>No gifts yet<br /></div>", "<span class='InnerCPanelText'>" & numGifts & IIf(numGifts > 1, " Gifts", " Gift"))
    End Function

    Public Function itemVisible(ByVal id As Object, ByVal tbl As Object, ByVal whereItem As Object) As Boolean

        Dim numItems As Integer

        Try
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            strSQL = "SELECT Count(*) FROM " & tbl & " WHERE " & whereItem & " = @ID"

            oCmd.Parameters.Clear()

            oParam = New SqlParameter()
            oParam.ParameterName = "ID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = id
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = strSQL
            oCmd.Connection.Open()

            numItems = oCmd.ExecuteScalar()

            Return (numItems > 0)

        Catch ex As Exception
            Throw ex
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If

            oCmd.Dispose()
        End Try

    End Function

    Public Function formatNullField(ByVal f As Object) As String
        Return IIf(f IsNot DBNull.Value Or f.ToString.Trim.Length > 0, f, "<i>None</i>")
    End Function

    Protected Sub backClick(ByVal sender As Object, ByVal e As System.EventArgs)
        Response.Redirect("Default.aspx")
    End Sub



End Class

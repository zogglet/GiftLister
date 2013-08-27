Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports AjaxControlToolkit

Partial Class _Default
    Inherits System.Web.UI.Page

    Dim oConn As New SqlConnection(ConfigurationManager.ConnectionStrings("GiftListConnectionString").ConnectionString)
    Dim oCmd As New SqlCommand
    Dim oParam As SqlParameter
    Dim oDA As New SqlDataAdapter
    Dim oDTbl As New DataTable
    Dim inner_dTbl As New DataTable
    Dim gifts_dTbl As New DataTable
    Dim strSQL As String = ""

    Dim msgScript As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            Session("NameSortDir") = SortDirection.Ascending
            Session("GiftCompleteSortDir") = SortDirection.Ascending
            Session("DescSortDir") = SortDirection.Ascending

            bindCategories()

            configTimerTimes()
            Session("Visible") = True

            'Set this so that something is shown right off the bat
            configCountdown()

        End If

    End Sub

    'Click event is generated in JavaScript on page load
    Protected Sub postback_btn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles postback_btn.Click
        cat_updatePnl.Update()
    End Sub

    Private Sub configTimerTimes()
        Session("Christmas") = "12/25/" & IIf(DateTime.Today <= "12/25/" & DateTime.Now.Year, DateTime.Now.Year, DateTime.Now.Year + 1)

        Session("SecondsRemaining") = DateDiff(DateInterval.Second, DateTime.Now, Session("Christmas"))
        Session("DaysRemaining") = Session("SecondsRemaining") / 86400
        Session("HoursRemaining") = Session("SecondsRemaining") / 3600
        Session("MinutesRemaining") = Session("SecondsRemaining") / 60

    End Sub

    Protected Sub countdown_timer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles countdown_timer.Tick

        Session("SecondsRemaining") -= 1

        If Session("SecondsRemaining") Mod 60 = 59 Then
            Session("MinutesRemaining") -= 1
        End If

        If Session("MinutesRemaining") Mod 60 = 59 Then
            Session("HoursRemaining") -= 1
        End If

        If Session("HoursRemaining") Mod 24 = 23 Then
            Session("DaysRemaining") -= 1
        End If

        'Configure how time is displayed based on time left
        configCountdown()

    End Sub

    Private Sub configCountdown()
        Dim countdownStr As String = ""

        Select Case Session("SecondsRemaining")
            'If the time is any time during Christmas
            Case -86399 To 0
                'On Christmas, display "Merry Christmas" message until the end of the day (monitored by xmas_timer), then start countdown again
                countdown_timer.Enabled = False

                countdownStr = "<span class='OverWeekStyle'>Merry Christmas!</span><br /><span class='UnderWeekStyle'>"

                Session("XMasTimeRemaining") = DateDiff(DateInterval.Second, DateTime.Now, CDate("#" & Today.Date & " 11:59:59 PM#")) + 1
                xmas_timer.Enabled = True

            Case 1 To 86400
                'Under a day left
                Session("Visible") = IIf(Session("Visible") = False, True, False)
                countdownStr = "<span class='UnderWeekStyle'" & IIf(Session("Visible"), " style='visibility:hidden;'>", ">")
            Case 86401 To 604800
                'Under a week left
                countdownStr = "<span class='UnderWeekStyle'>"
            Case Is > 604800
                'Over a week left
                countdownStr = "<span class='OverWeekStyle'>"
        End Select

        countdownStr &= IIf(Session("SecondsRemaining") >= -86399 And Session("SecondsRemaining") <= 0, "000:00:00:00", configZeroes(Session("DaysRemaining"), "00") & ":" & configZeroes(Session("HoursRemaining") Mod 24) & ":" & configZeroes(Session("MinutesRemaining") Mod 60) & ":" & configZeroes(Session("SecondsRemaining") Mod 60) & "</span>")

        countdown_lit.Text = countdownStr
        countdown_updatePnl.Update()

    End Sub

    Protected Sub xmas_timer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles xmas_timer.Tick

        Session("XMasTimeRemaining") -= 1


        If Session("XMasTimeRemaining") = 0 Then
            xmas_timer.Enabled = False

            configTimerTimes()
            countdown_timer.Enabled = True
        Else
            'Only resetting this each time so that I can toggle the styling
            Session("Visible") = IIf(Session("Visible") = False, True, False)

            countdown_lit.Text = "<span class='OverWeekStyle'>Merry Christmas!</span><br /><span class='UnderWeekStyle'" & IIf(Session("Visible"), " style='visibility:hidden;'>", ">") & "000:00:00:00</span>"

        End If

    End Sub

    Protected Function configZeroes(ByVal val As Integer, Optional ByVal zeroesToAdd As String = "0") As String
        Return IIf(val < 10, zeroesToAdd & val, val)
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

                strSQL = "SELECT *, Title FROM Recipients INNER JOIN Categories on Recipients.CategoryID = Categories.ID WHERE CategoryID = @CategoryID ORDER BY LastName, FirstName"

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

                    Dim cbx As CheckBox = CType(gv.Rows(j).FindControl("recipientComplete_cbx"), CheckBox)
                    cbx.Enabled = (gifts_dTbl.Rows.Count > 0)

                    Dim checkmark_img As Image = CType(cat_repeater.Items(i).FindControl("checkmark_img"), Image)
                    Dim catID As Integer = CType(cat_repeater.Items(i).FindControl("id_hField"), HiddenField).Value

                    checkmark_img.Visible = getCategoryCompleteStatus(catID)

                    For k As Integer = 0 To gifts_dTbl.Rows.Count - 1
                        Dim inner_cbx As CheckBox = CType(inner_gv.Rows(k).FindControl("giftComplete_cbx"), CheckBox)
                        Session("Gift_" & inner_cbx.ClientID & "_State") = inner_cbx.Checked
                        Session(inner_gv.ClientID & "_AllChecked") = allGiftCheckboxesChecked(inner_gv)
                    Next

                Next

            Next

            print_pnl.Visible = (cat_repeater.Items.Count > 0)
            cat_updatePnl.Update()

        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Protected Sub printClick(ByVal sender As Object, ByVal e As System.EventArgs)
        Session("PrintMode") = print_rbList.SelectedValue
        Session("ListTitle") = IIf(listTitle_txt.Text.Trim.Length > 0, listTitle_txt.Text.Trim, Nothing)
        Session("PrintImages") = printImgs_cbx.Checked
        Response.Redirect("Print.aspx")
    End Sub

    Protected Sub recipientCompleteCheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim gv As GridView
        Dim inner_gv As GridView
        Dim row As GridViewRow
        Dim id As Integer
        Dim catID As Integer
        Dim rowStatus As String
        Dim checkmark_img As Image

        Try

            'Isolate recipient's ID, the category ID, the row to style, and the checkmark image in the Repeater to show/hide (better than using parent)
            For i As Integer = 0 To cat_repeater.Items.Count - 1

                gv = CType(cat_repeater.Items(i).FindControl("recipients_gv"), GridView)

                For j As Integer = 0 To gv.Rows.Count - 1
                    If CType(gv.Rows(j).FindControl("recipientComplete_cbx"), CheckBox) Is sender Then
                        id = gv.DataKeys(j).Values("ID")
                        catID = CType(cat_repeater.Items(i).FindControl("id_hField"), HiddenField).Value
                        checkmark_img = CType(cat_repeater.Items(i).FindControl("checkmark_img"), Image)
                        row = gv.Rows(j)
                        inner_gv = CType(gv.Rows(j).FindControl("gifts_gv"), GridView)

                        GoTo EndLoop
                    End If
                Next
            Next

EndLoop:
         
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            'If we're unchecking the recipients checkbox while Retain Child States is selected, and if all checkboxes have
            'not been individually checked, bind each row according to the session variable.
            If retainGiftStates_cbx.Checked And CType(sender, CheckBox).Checked = False And Session(inner_gv.ClientID & "_AllChecked") = False Then
                For i As Integer = 0 To cat_repeater.Items.Count - 1
                    gv = CType(cat_repeater.Items(i).FindControl("recipients_gv"), GridView)

                    For j As Integer = 0 To gv.Rows.Count - 1
                        If CType(gv.Rows(j).FindControl("recipientComplete_cbx"), CheckBox) Is sender Then

                            strSQL = "UPDATE Gifts SET Complete = @Complete WHERE ID = @ID"

                            For k As Integer = 0 To inner_gv.Rows.Count - 1
                                Dim inner_cbx = CType(inner_gv.Rows(k).FindControl("giftComplete_cbx"), CheckBox)

                                oCmd.Parameters.Clear()

                                oParam = New SqlParameter()
                                oParam.ParameterName = "Complete"
                                oParam.SqlDbType = SqlDbType.Bit
                                oParam.Value = Session("Gift_" & inner_cbx.ClientID & "_State")
                                oCmd.Parameters.Add(oParam)

                                oParam = New SqlParameter()
                                oParam.ParameterName = "ID"
                                oParam.SqlDbType = SqlDbType.Int
                                oParam.Value = inner_gv.DataKeys(k).Values("ID")
                                oCmd.Parameters.Add(oParam)

                                oCmd.CommandText = strSQL

                                oCmd.Connection.Open()
                                oCmd.ExecuteScalar()
                                oCmd.Connection.Close()
                            Next

                            Exit For

                        End If

                    Next
                Next
            Else
                'In all other cases, update the whole table according to recipientID and the outer checkbox value
                strSQL = "UPDATE Gifts SET Complete = @Complete WHERE RecipientID = @RecipientID"

                oCmd.Parameters.Clear()

                oParam = New SqlParameter()
                oParam.ParameterName = "Complete"
                oParam.SqlDbType = SqlDbType.Bit
                oParam.Value = CType(sender, CheckBox).Checked
                oCmd.Parameters.Add(oParam)

                oParam = New SqlParameter()
                oParam.ParameterName = "RecipientID"
                oParam.SqlDbType = SqlDbType.Int
                oParam.Value = id
                oCmd.Parameters.Add(oParam)

                oCmd.CommandText = strSQL

                oCmd.Connection.Open()
                oCmd.ExecuteScalar()
                oCmd.Connection.Close()
            End If



            '************************************************** 

            'Check all inner checkboxes and style inner GridView rows
            For i As Integer = 0 To cat_repeater.Items.Count - 1

                gv = CType(cat_repeater.Items(i).FindControl("recipients_gv"), GridView)

                For j As Integer = 0 To gv.Rows.Count - 1

                    If CType(gv.Rows(j).FindControl("recipientComplete_cbx"), CheckBox) Is sender Then
                        inner_gv = CType(gv.Rows(j).FindControl("gifts_gv"), GridView)
                        For k As Integer = 0 To inner_gv.Rows.Count - 1

                            Dim inner_cbx = CType(inner_gv.Rows(k).FindControl("giftComplete_cbx"), CheckBox)
                            inner_cbx.Checked = IIf(CType(sender, CheckBox).Checked = False And retainGiftStates_cbx.Checked And Session(inner_gv.ClientID & "_AllChecked") = False, Session("Gift_" & inner_cbx.ClientID & "_State"), CType(sender, CheckBox).Checked)

                            CType(inner_gv.Rows(k), GridViewRow).CssClass = IIf(inner_cbx.Checked, "GVInnerCompleteStyle", "GVInnerItemStyle")
                            CType(inner_gv.Rows(k).FindControl("tooltip_pnl"), Panel).CssClass = getToolTipClass(Session("Gift_" & inner_cbx.ClientID & "_State"))
                        Next
                    End If

                Next
            Next

            rowStatus = getRecipientCompleteStatus(id)

            'Style outer GridView row
            For l As Integer = 0 To row.Cells.Count - 1
                If l <> 1 Then
                    row.Cells(l).CssClass = IIf(rowStatus = "Complete", "GVItemCompleteStyle", IIf(rowStatus = "Partial", "GVItemPartialStyle", "GVNameStyle"))
                Else
                    'Style inner cell
                    row.Cells(l).BackColor = IIf(rowStatus = "Complete", Drawing.ColorTranslator.FromHtml("#958e5b"), IIf(rowStatus = "Partial", Drawing.ColorTranslator.FromHtml("#c9b370"), Nothing))
                End If
            Next

            'Show/hide category checkmark
            checkmark_img.Visible = getCategoryCompleteStatus(catID)

        Catch ex As Exception
            Throw ex
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If

            oCmd.Dispose()
        End Try

    End Sub


    Protected Sub giftCompleteCheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim row As GridViewRow
        Dim gv As GridView
        Dim inner_gv As GridView
        Dim outerCbx As CheckBox
        Dim id As Integer
        Dim recipID As Integer
        Dim catID As Integer
        Dim rowStatus As String
        Dim recipRow As GridViewRow
        Dim checkmark_img As Image
        Dim tooltip_pnl As Panel

        Try

            'Isolate the Gifts ID, the RecipientID, the outer Gridview (recipient) row, the inner GridView row, and other items
            'required for the configuration of the checklist upon checkchanged of a gift
            For i As Integer = 0 To cat_repeater.Items.Count - 1

                gv = CType(cat_repeater.Items(i).FindControl("recipients_gv"), GridView)

                For j As Integer = 0 To gv.Rows.Count - 1

                    inner_gv = CType(gv.Rows(j).FindControl("gifts_gv"), GridView)
                    For k As Integer = 0 To inner_gv.Rows.Count - 1

                        Dim cbx As CheckBox = CType(inner_gv.Rows(k).FindControl("giftComplete_cbx"), CheckBox)
                        'I have to set this variable for all inner checkboxes, not just the sender, upon check/uncheck of a gift
                        Session("Gift_" & cbx.ClientID & "_State") = cbx.Checked
                        Session(inner_gv.ClientID & "_AllChecked") = allGiftCheckboxesChecked(inner_gv)

                        If CType(inner_gv.Rows(k).FindControl("giftComplete_cbx"), CheckBox) Is sender Then
                            id = inner_gv.DataKeys(k).Values("ID")
                            recipID = inner_gv.DataKeys(k).Values("RecipientID")
                            catID = CType(cat_repeater.Items(i).FindControl("id_hField"), HiddenField).Value
                            row = inner_gv.Rows(k)
                            tooltip_pnl = CType(inner_gv.Rows(k).FindControl("tooltip_pnl"), Panel)
                            recipRow = gv.Rows(j)
                            outerCbx = CType(gv.Rows(j).FindControl("recipientComplete_cbx"), CheckBox)
                            checkmark_img = CType(cat_repeater.Items(i).FindControl("checkmark_img"), Image)
                            GoTo EndLoop
                        End If

                    Next
                Next
            Next

EndLoop:

            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            strSQL = "UPDATE Gifts SET Complete = @Complete WHERE ID = @ID"

            oCmd.Parameters.Clear()

            oParam = New SqlParameter()
            oParam.ParameterName = "Complete"
            oParam.SqlDbType = SqlDbType.Bit
            oParam.Value = CType(sender, CheckBox).Checked
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter()
            oParam.ParameterName = "ID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = id
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = strSQL
            oCmd.Connection.Open()

            oCmd.ExecuteScalar()
            oCmd.Connection.Close()

            '**********************************************

            'Uncheck or check outer checkbox
            outerCbx.Checked = allGiftCheckboxesChecked(inner_gv)

            'Style inner GridView row and tooltip
            row.CssClass = IIf(CType(sender, CheckBox).Checked, "GVInnerCompleteStyle", "GVInnerItemStyle")

            tooltip_pnl.CssClass = getToolTipClass(CType(sender, CheckBox).Checked)

            rowStatus = getRecipientCompleteStatus(recipID)

            'If checking the inner checkbox completes all gifts for a recipient (rowStatus), style the whole outer row
            For k As Integer = 0 To recipRow.Cells.Count - 1
                If k <> 1 Then
                    recipRow.Cells(k).CssClass = IIf(rowStatus = "Complete", "GVItemCompleteStyle", IIf(rowStatus = "Partial", "GVItemPartialStyle", "GVNameStyle"))
                Else
                    'Style inner cell
                    recipRow.Cells(k).BackColor = IIf(rowStatus = "Complete", Drawing.ColorTranslator.FromHtml("#958e5b"), IIf(rowStatus = "Partial", Drawing.ColorTranslator.FromHtml("#c9b370"), Nothing))
                End If
            Next

            checkmark_img.Visible = getCategoryCompleteStatus(catID)

        Catch ex As Exception
            Throw ex
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If

            oCmd.Dispose()
        End Try
    End Sub

    Protected Sub giftsRowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)

        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.CssClass = IIf(e.Row.DataItem("Complete") = True, "GVInnerCompleteStyle", "GVInnerItemStyle")
        End If

    End Sub

    Protected Sub recipientSelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim gv As GridView = CType(sender, GridView)

        Session("SelectedRecipient") = gv.DataKeys(gv.SelectedIndex).Values("ID")
        Session("SelectedRecipientCategory") = gv.DataKeys(gv.SelectedIndex).Values("CategoryID")

        configFormView("Recipient")
        fv_updatePnl.Update()
        fv_mpExt.Show()

    End Sub

    Protected Sub giftSelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim gv As GridView = CType(sender, GridView)

        Session("SelectedGift") = gv.DataKeys(gv.SelectedIndex).Values("ID")
        Session("SelectedGiftRecipient") = gv.DataKeys(gv.SelectedIndex).Values("RecipientID")

        configFormView("Gift")
        fv_updatePnl.Update()
        fv_mpExt.Show()
    End Sub

    Protected Sub addCategoryClick(ByVal sender As Object, ByVal e As System.EventArgs)
        category_fv.ChangeMode(FormViewMode.Insert)
        configFormView("Category")
        fv_updatePnl.Update()
        fv_mpExt.Show()
    End Sub

    Protected Sub editCategoryClick(ByVal sender As Object, ByVal e As System.EventArgs)
        For i As Integer = 0 To cat_repeater.Items.Count - 1
            If CType(cat_repeater.Items(i).FindControl("editCategory_btn"), Button) Is sender Then
                Session("SelectedCategory") = CType(cat_repeater.Items(i).FindControl("id_hField"), HiddenField).Value

                configFormView("Category")
                fv_updatePnl.Update()
                fv_mpExt.Show()
                Exit For
            End If
        Next
    End Sub

    Protected Sub addRecipientClick(ByVal sender As Object, ByVal e As System.EventArgs)

        For i As Integer = 0 To cat_repeater.Items.Count - 1

            If CType(cat_repeater.Items(i).FindControl("addRecipient_btn"), Button) Is sender Then

                Session("SelectedRecipientCategory") = CType(cat_repeater.Items(i).FindControl("id_hField"), HiddenField).Value
                recipient_fv.ChangeMode(FormViewMode.Insert)
                configFormView("Recipient")
                fv_updatePnl.Update()
                fv_mpExt.Show()
                Exit For
            End If
        Next


    End Sub

    Protected Sub addGiftClick(ByVal sender As Object, ByVal e As System.EventArgs)

        For i As Integer = 0 To cat_repeater.Items.Count - 1

            Dim gv As GridView = CType(cat_repeater.Items(i).FindControl("recipients_gv"), GridView)

            For j As Integer = 0 To gv.Rows.Count - 1

                If CType(gv.Rows(j).FindControl("addGift_btn"), Button) Is sender Then
                    Session("SelectedGiftRecipient") = gv.DataKeys(j).Values("ID")
                    gift_fv.ChangeMode(FormViewMode.Insert)
                    configFormView("Gift")
                    fv_updatePnl.Update()
                    fv_mpExt.Show()
                    Exit For
                End If

            Next

        Next

    End Sub

    Protected Sub manageImagesClick(ByVal sender As Object, ByVal e As System.EventArgs)

        For i As Integer = 0 To cat_repeater.Items.Count - 1
            Dim gv As GridView = CType(cat_repeater.Items(i).FindControl("recipients_gv"), GridView)

            For j As Integer = 0 To gv.Rows.Count - 1

                Dim inner_gv As GridView = CType(gv.Rows(j).FindControl("gifts_gv"), GridView)

                For k As Integer = 0 To inner_gv.Rows.Count - 1
                    If CType(inner_gv.Rows(k).FindControl("images_lBtn"), LinkButton) Is sender Then
                        Session("SelectedImagesGift") = inner_gv.DataKeys(k).Values("ID")
                        'Set so I can use this in the header of the Images page
                        Session("SelectedGiftRecipient") = inner_gv.DataKeys(k).Values("RecipientID")
                        GoTo EndLoop
                    End If

                Next
            Next
        Next

EndLoop:
        Response.Redirect("Images.aspx")

    End Sub

    Private Sub configFormView(ByVal mode As String)

        gift_fv.Visible = (mode = "Gift")
        recipient_fv.Visible = (mode = "Recipient")
        category_fv.Visible = (mode = "Category")
    End Sub

    'Not just checking the checkboxes, but updating via the FormView will make these changes
    Protected Sub recipientsRowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)

        Dim rowStatus As String
        Dim cb As CheckBox = CType(e.Row.FindControl("recipientComplete_cbx"), CheckBox)

        If e.Row.RowType = DataControlRowType.DataRow Then

            'Configure outer checkbox and row style
            rowStatus = getRecipientCompleteStatus(CType(sender, GridView).DataKeys(e.Row.RowIndex).Values("ID"))
            cb.Checked = (rowStatus = "Complete")

            'Only the rows not including the first and last
            For i As Integer = 0 To e.Row.Cells.Count - 1
                If i <> 1 Then
                    e.Row.Cells(i).CssClass = IIf(rowStatus = "Complete", "GVItemCompleteStyle", IIf(rowStatus = "Partial", "GVItemPartialStyle", "GVNameStyle"))
                Else
                    e.Row.Cells(i).BackColor = IIf(rowStatus = "Complete", Drawing.ColorTranslator.FromHtml("#958e5b"), IIf(rowStatus = "Partial", Drawing.ColorTranslator.FromHtml("#c9b370"), Nothing))
                End If

            Next
        End If

    End Sub

    Private Function getCategoryCompleteStatus(ByVal catID As Integer) As Boolean
        Dim numGiftsForCategory As Integer
        Dim numCompleteGiftsForCategory As Integer
        Dim tempStrSQL As String = ""

        Try

            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            'Get all gifts for category
            tempStrSQL = "SELECT COUNT(*) FROM (SELECT Gifts.ID AS GiftID FROM Recipients INNER JOIN Gifts ON Recipients.ID = Gifts.RecipientID WHERE Recipients.CategoryID = @CategoryID) AS CategoryGifts"

            oCmd.Parameters.Clear()

            oParam = New SqlParameter()
            oParam.ParameterName = "CategoryID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = catID
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = tempStrSQL

            oCmd.Connection.Open()
            numGiftsForCategory = oCmd.ExecuteScalar()

            'Get complete gifts for category
            tempStrSQL = "SELECT COUNT(*) FROM (SELECT Gifts.ID AS GiftID FROM Recipients INNER JOIN Gifts ON Recipients.ID = Gifts.RecipientID WHERE Recipients.CategoryID = @CategoryID AND Gifts.Complete = 1) AS CategoryGifts"

            oCmd.Parameters.Clear()

            oParam = New SqlParameter()
            oParam.ParameterName = "CategoryID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = catID
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = tempStrSQL

            numCompleteGiftsForCategory = oCmd.ExecuteScalar()

            Return (numCompleteGiftsForCategory = numGiftsForCategory And numGiftsForCategory <> 0)

        Catch ex As Exception
            Throw ex
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If

            oCmd.Dispose()
        End Try


        'Old, non-working approach:
        'Try
        '    oCmd.Connection = oConn
        '    oCmd.CommandType = CommandType.Text

        '    'Query 1: Get all recipients for category
        '    strSQL = "SELECT ID FROM Recipients WHERE CategoryID = @CategoryID"

        '    oCmd.Parameters.Clear()
        '    oDTbl.Clear()

        '    oParam = New SqlParameter()
        '    oParam.ParameterName = "CategoryID"
        '    oParam.SqlDbType = SqlDbType.Int
        '    oParam.Value = catID
        '    oCmd.Parameters.Add(oParam)

        '    oCmd.CommandText = strSQL
        '    oDA.SelectCommand = oCmd

        '    oDA.Fill(oDTbl)

        '    If oDTbl.Rows.Count > 0 Then
        '        giftsSelectStr &= "WHERE "
        '    End If
        '    'Query 2: Select gifts where the recipientID is that of any of the recipients selected by category
        '    For i As Integer = 0 To oDTbl.Rows.Count - 1
        '        giftsSelectStr &= "RecipientID = '" & oDTbl.Rows(i)("ID") & "'" & IIf(i < oDTbl.Rows.Count - 1, " OR ", "")
        '    Next

        '    oCmd.Parameters.Clear()

        '    oCmd.CommandText = giftsSelectStr
        '    oCmd.Connection.Open()

        '    numGiftsForCategory = oCmd.ExecuteScalar()

        '    'Query 3: Get completed gifts for the category (use already-established initial string)
        '    completeGiftsSelectStr = giftsSelectStr & IIf(oDTbl.Rows.Count > 0, " AND ", " WHERE ") & "Complete = '1'"

        '    oCmd.CommandText = completeGiftsSelectStr

        '    numCompleteGiftsForCategory = oCmd.ExecuteScalar()

        '    Return (numCompleteGiftsForCategory = numGiftsForCategory And numGiftsForCategory <> 0)

        'Catch ex As Exception
        '    Throw ex
        'Finally
        '    If oConn.State = ConnectionState.Open Then
        '        oConn.Close()
        '    End If

        '    oCmd.Dispose()
        'End Try
    End Function

    Private Function getRecipientCompleteStatus(ByVal recipID As Integer) As String

        Dim numTotalGifts As Integer
        Dim numCompleteGifts As Integer
        Dim status As String
        Dim tempStrSQL As String = ""

        Try
            'Select total gifts
            numTotalGifts = giftsForRecipient(recipID)

            'Select completed gifts
            tempStrSQL = "SELECT COUNT(*) FROM Gifts WHERE RecipientID = @RecipientID AND Complete = 1"

            oParam = New SqlParameter()
            oParam.ParameterName = "RecipientID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = recipID
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = tempStrSQL
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

    Private Function allGiftCheckboxesChecked(ByVal sender As CheckBox) As Boolean

        Dim inner_gv As GridView

        For i As Integer = 0 To cat_repeater.Items.Count - 1

            Dim gv As GridView = CType(cat_repeater.Items(i).FindControl("recipients_gv"), GridView)

            For j As Integer = 0 To gv.Rows.Count - 1

                inner_gv = CType(gv.Rows(j).FindControl("gifts_gv"), GridView)

                For k As Integer = 0 To inner_gv.Rows.Count - 1

                    'Establish inner_gv as the chosen GridView to check
                    If CType(inner_gv.Rows(k).FindControl("giftComplete_cbx"), CheckBox) Is sender Then

                        'If any inner checkboxes are checked, the recipient is not complete, so return false
                        For l As Integer = 0 To inner_gv.Rows.Count - 1
                            If CType(inner_gv.Rows(l).FindControl("giftComplete_cbx"), CheckBox).Checked = False Then
                                Return False
                            End If
                        Next

                        Return True
                    End If

                Next
            Next
        Next
    End Function

    'For passing in the inner GridView (to eliminate the extra loop)
    Private Function allGiftCheckboxesChecked(ByVal sender As GridView) As Boolean

        'If any inner checkboxes are checked, the recipient is not complete, so return false
        For i As Integer = 0 To sender.Rows.Count - 1
            If CType(sender.Rows(i).FindControl("giftComplete_cbx"), CheckBox).Checked = False Then
                Return False
            End If
        Next

        Return True

    End Function

    Private Function numItemsForThis(ByVal id As Integer, ByVal tbl As String, ByVal whereItem As String) As Integer
        Dim tempStrSQL As String = ""

        Try

        Catch ex As Exception

        End Try
    End Function

    Private Function imagesForGift(ByVal giftID As Integer) As Integer
        Dim tempStrSQL As String = ""

        Try
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            tempStrSQL = "SELECT Count(*) FROM Images WHERE GiftID = @ID"

            oCmd.Parameters.Clear()

            oParam = New SqlParameter()
            oParam.ParameterName = "ID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = giftID
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = tempStrSQL
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

    Private Function giftsForRecipient(ByVal recipID As Integer) As Integer
        Dim tempStrSQL As String = ""

        Try
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            tempStrSQL = "SELECT Count(*) FROM Gifts WHERE RecipientID = @ID"

            oCmd.Parameters.Clear()

            oParam = New SqlParameter()
            oParam.ParameterName = "ID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = recipID
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = tempStrSQL
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
        Dim tempStrSQL As String = ""

        Try
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            tempStrSQL = "SELECT Count(*) FROM Recipients WHERE CategoryID = @ID"

            oCmd.Parameters.Clear()

            oParam = New SqlParameter()
            oParam.ParameterName = "ID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = catID
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = tempStrSQL
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

    Public Function configCPText(ByVal title As Object, ByVal catID As Integer, ByVal expanded As Boolean) As String
        Dim numRecips As Integer = recipientsForCategory(catID)
        Return title & ": " & numRecips & IIf(numRecips > 1, " Recipients", " Recipient") & IIf(expanded, " [hide]", " [show]")
    End Function

    Public Function configEmptyCatText(ByVal title As Object) As String
        Return title & " [empty]"
    End Function

    Public Function configGiftsCPText(ByVal recipID As Object, ByVal expanded As Boolean) As String

        Dim numGifts = giftsForRecipient(recipID)
        Return numGifts & IIf(numGifts > 1, " Gifts", " Gift") & IIf(expanded, " [hide]", " [show]")
    End Function

    Public Function itemVisible(ByVal id As Object, ByVal tbl As Object, ByVal whereItem As Object, ByVal showForEmpty As Boolean) As Boolean

        Dim numItems As Integer
        Dim tempStrSQL As String = ""

        Try
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            tempStrSQL = "SELECT Count(*) FROM " & tbl & " WHERE " & whereItem & " = @ID"

            oCmd.Parameters.Clear()

            oParam = New SqlParameter()
            oParam.ParameterName = "ID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = id
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = tempStrSQL
            oCmd.Connection.Open()

            numItems = oCmd.ExecuteScalar()

            Return IIf(showForEmpty, (numItems = 0), (numItems > 0))

        Catch ex As Exception
            Throw ex
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If

            oCmd.Dispose()
        End Try

    End Function

    Public Function formatImagesText(ByVal id As Object) As String
        Dim numImages As Integer = imagesForGift(id)
        Return IIf(numImages > 0, "Manage Images (" & numImages & ")", "Add Images")
    End Function

    Public Function formatCatHeader() As String
        Try
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            strSQL = "SELECT Title FROM Categories WHERE ID = @ID"

            oCmd.Parameters.Clear()

            oParam = New SqlParameter
            oParam.ParameterName = "ID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = Session("SelectedRecipientCategory")
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = strSQL

            oCmd.Connection.Open()
            Return "<h3>Add/Edit Recipient in " & oCmd.ExecuteScalar() & "</h3>"
        Catch ex As Exception
            Throw ex

        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If

            oCmd.Dispose()
        End Try
    End Function

    Public Function formatRecipHeader() As String

        Try
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            strSQL = "SELECT FirstName, LastName FROM Recipients WHERE ID = @ID"

            oCmd.Parameters.Clear()

            oParam = New SqlParameter
            oParam.ParameterName = "ID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = Session("SelectedGiftRecipient")
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = strSQL

            oDA.SelectCommand = oCmd
            oDA.Fill(oDTbl)


            Return "<h3>Add/Edit Gift for " & oDTbl.Rows(0)("FirstName") & " " & IIf(oDTbl.Rows(0)("LastName") IsNot DBNull.Value, oDTbl.Rows(0)("LastName"), "") & "</h3>"
        Catch ex As Exception
            Throw ex
        End Try

    End Function

    Public Function formatGiftHeader() As String
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

            Return "<h3>Add/Edit Images for " & oCmd.ExecuteScalar() & "</h3>"
        Catch ex As Exception
            Throw ex
        Finally

            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If

            oCmd.Dispose()
        End Try
    End Function

    Public Function formatCompleteText(ByVal complete As Object) As String
        Return IIf(complete = True, "Yes", "No")
    End Function

    Public Function formatNullField(ByVal f As Object) As String
        Return IIf(f IsNot DBNull.Value, f, "<i>Not specified.</i>")
    End Function

    Public Function isToolTipVisible(ByVal notes As Object) As String
        Return (notes IsNot DBNull.Value)
    End Function

    Public Function formatToolTipText(ByVal notes As Object) As String
        Return "<b>Notes:</b><br />" & notes
    End Function

    Public Function getToolTipClass(ByVal complete As Object) As String
        Return IIf(complete = True, "ToolTipCompleteStyle", "ToolTipIncompleteStyle")
    End Function

    'For sorting
    Private Sub bindRecipsGV(ByVal sender As Object, ByVal linkButtonID As String, Optional ByVal orderBy As String = "LastName ASC, FirstName ASC")
        Dim catID As Integer
        Dim gv As GridView
        Dim checkmark_img As Image

        For i As Integer = 0 To cat_repeater.Items.Count - 1
            'This will save the correct GridView, because the loop will exit when the proper CategoryID is found :]
            gv = CType(cat_repeater.Items(i).FindControl("recipients_gv"), GridView)

            For j As Integer = 0 To gv.Rows.Count - 1
                If gv.HeaderRow IsNot Nothing Then
                    If sender Is gv.HeaderRow.FindControl(linkButtonID) Then
                        catID = gv.DataKeys(j).Values("CategoryID")
                        checkmark_img = CType(cat_repeater.Items(i).FindControl("checkmark_img"), Image)
                        GoTo EndLoop
                    End If
                End If

            Next
        Next

EndLoop:

        oCmd.Connection = oConn
        oCmd.CommandType = CommandType.Text

        strSQL = "SELECT *, Title FROM Recipients INNER JOIN Categories on Recipients.CategoryID = Categories.ID WHERE CategoryID = @CategoryID ORDER BY " & orderBy

        oCmd.Parameters.Clear()
        inner_dTbl.Clear()

        oParam = New SqlParameter
        oParam.ParameterName = "CategoryID"
        oParam.SqlDbType = SqlDbType.Int
        oParam.Value = catID
        oCmd.Parameters.Add(oParam)

        oCmd.CommandText = strSQL
        oDA.SelectCommand = oCmd

        oDA.Fill(inner_dTbl)

        gv.DataSource = inner_dTbl
        gv.DataBind()

        'Bind inner GridView too
        For k As Integer = 0 To inner_dTbl.Rows.Count - 1

            Dim inner_gv As GridView = CType(gv.Rows(k).FindControl("gifts_gv"), GridView)

            oCmd.Parameters.Clear()
            gifts_dTbl.Clear()

            strSQL = "SELECT * FROM Gifts WHERE RecipientID = @RecipientID ORDER BY Description"

            oParam = New SqlParameter
            oParam.ParameterName = "RecipientID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = inner_dTbl.Rows(k)("ID")
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = strSQL
            oDA.SelectCommand = oCmd

            oDA.Fill(gifts_dTbl)

            inner_gv.DataSource = gifts_dTbl
            inner_gv.DataBind()

            For l As Integer = 0 To gifts_dTbl.Rows.Count - 1
                Dim inner_cbx As CheckBox = CType(inner_gv.Rows(l).FindControl("giftComplete_cbx"), CheckBox)
                Session("Gift_" & inner_cbx.ClientID & "_State") = inner_cbx.Checked
                Session(inner_gv.ClientID & "_AllChecked") = allGiftCheckboxesChecked(inner_gv)

            Next

            Dim cbx As CheckBox = CType(gv.Rows(k).FindControl("recipientComplete_cbx"), CheckBox)
            cbx.Enabled = (gifts_dTbl.Rows.Count > 0)

        Next

        checkmark_img.Visible = getCategoryCompleteStatus(catID)

        cat_updatePnl.Update()
    End Sub

    Private Sub bindRecipsGV()

        Dim catID As Integer
        Dim gv As GridView
        Dim checkmark_img As Image

        For i As Integer = 0 To cat_repeater.Items.Count - 1
            'This will save the correct GridView, because the loop will exit when the proper CategoryID is found :]
            gv = CType(cat_repeater.Items(i).FindControl("recipients_gv"), GridView)

            For j As Integer = 0 To gv.Rows.Count - 1
                If gv.DataKeys(j).Values("ID") = Session("SelectedGiftRecipient") Then
                    catID = gv.DataKeys(j).Values("CategoryID")
                    checkmark_img = CType(cat_repeater.Items(i).FindControl("checkmark_img"), Image)
                    GoTo EndLoop
                End If
            Next
        Next

EndLoop:

        oCmd.Connection = oConn
        oCmd.CommandType = CommandType.Text

        strSQL = "SELECT *, Title FROM Recipients INNER JOIN Categories on Recipients.CategoryID = Categories.ID WHERE CategoryID = @CategoryID ORDER BY LastName, FirstName"

        oCmd.Parameters.Clear()
        inner_dTbl.Clear()

        oParam = New SqlParameter
        oParam.ParameterName = "CategoryID"
        oParam.SqlDbType = SqlDbType.Int
        oParam.Value = catID
        oCmd.Parameters.Add(oParam)

        oCmd.CommandText = strSQL
        oDA.SelectCommand = oCmd

        oDA.Fill(inner_dTbl)

        gv.DataSource = inner_dTbl
        gv.DataBind()

        'Bind inner GridView too
        For k As Integer = 0 To inner_dTbl.Rows.Count - 1

            Dim inner_gv As GridView = CType(gv.Rows(k).FindControl("gifts_gv"), GridView)

            oCmd.Parameters.Clear()
            gifts_dTbl.Clear()

            strSQL = "SELECT * FROM Gifts WHERE RecipientID = @RecipientID ORDER BY Description"

            oParam = New SqlParameter
            oParam.ParameterName = "RecipientID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = inner_dTbl.Rows(k)("ID")
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = strSQL
            oDA.SelectCommand = oCmd

            oDA.Fill(gifts_dTbl)

            inner_gv.DataSource = gifts_dTbl
            inner_gv.DataBind()

            For l As Integer = 0 To gifts_dTbl.Rows.Count - 1
                Dim inner_cbx As CheckBox = CType(inner_gv.Rows(l).FindControl("giftComplete_cbx"), CheckBox)
                Session("Gift_" & inner_cbx.ClientID & "_State") = inner_cbx.Checked
                Session(inner_gv.ClientID & "_AllChecked") = allGiftCheckboxesChecked(inner_gv)

            Next

            Dim cbx As CheckBox = CType(gv.Rows(k).FindControl("recipientComplete_cbx"), CheckBox)
            cbx.Enabled = (gifts_dTbl.Rows.Count > 0)

        Next

        checkmark_img.Visible = getCategoryCompleteStatus(catID)

        cat_updatePnl.Update()
    End Sub

    'For sorting
    Private Sub bindGiftsGV(ByVal sender As Object, ByVal linkButtonID As String, Optional ByVal orderBy As String = "Description ASC")
        Dim inner_gv As GridView
        Dim checkmark_img As Image
        Dim outerCbx As CheckBox
        Dim catID As Integer
        Dim recipID As Integer
        Dim rowStatus As String
        Dim recipRow As GridViewRow

        'Bind just this inner GridView
        For i As Integer = 0 To cat_repeater.Items.Count - 1

            Dim gv As GridView = CType(cat_repeater.Items(i).FindControl("recipients_gv"), GridView)

            For j As Integer = 0 To gv.Rows.Count - 1

                inner_gv = CType(gv.Rows(j).FindControl("gifts_gv"), GridView)

                If inner_gv.HeaderRow IsNot Nothing Then
                    If sender Is inner_gv.HeaderRow.FindControl(linkButtonID) Then
                        recipRow = gv.Rows(j)
                        outerCbx = CType(gv.Rows(j).FindControl("recipientComplete_cbx"), CheckBox)
                        catID = CType(cat_repeater.Items(i).FindControl("id_hField"), HiddenField).Value
                        recipID = gv.DataKeys(j).Values("ID")
                        checkmark_img = CType(cat_repeater.Items(i).FindControl("checkmark_img"), Image)

                        oCmd.Connection = oConn
                        oCmd.CommandType = CommandType.Text

                        oCmd.Parameters.Clear()
                        gifts_dTbl.Clear()

                        strSQL = "SELECT * FROM Gifts WHERE RecipientID = @RecipientID ORDER BY " & orderBy

                        oParam = New SqlParameter
                        oParam.ParameterName = "RecipientID"
                        oParam.SqlDbType = SqlDbType.Int
                        oParam.Value = recipID
                        oCmd.Parameters.Add(oParam)

                        oCmd.CommandText = strSQL
                        oDA.SelectCommand = oCmd

                        oDA.Fill(gifts_dTbl)

                        inner_gv.DataSource = gifts_dTbl
                        inner_gv.DataBind()

                        For k As Integer = 0 To gifts_dTbl.Rows.Count - 1
                            Dim inner_cbx As CheckBox = CType(inner_gv.Rows(k).FindControl("giftComplete_cbx"), CheckBox)
                            Session("Gift_" & inner_cbx.ClientID & "_State") = inner_cbx.Checked
                            Session(inner_gv.ClientID & "_AllChecked") = allGiftCheckboxesChecked(inner_gv)
                        Next

                        Exit For

                    End If
                End If


            Next

        Next

        checkmark_img.Visible = getCategoryCompleteStatus(catID)

        rowStatus = getRecipientCompleteStatus(recipID)

        outerCbx.Checked = (rowStatus = "Complete")

        'If checking the inner checkbox completes all gifts for a recipient (rowStatus), style the whole outer row (since the outer GridView's databound
        'event isn't called, this needs to be done separately)
        For k As Integer = 0 To recipRow.Cells.Count - 1
            If k <> 1 Then
                recipRow.Cells(k).CssClass = IIf(rowStatus = "Complete", "GVItemCompleteStyle", IIf(rowStatus = "Partial", "GVItemPartialStyle", "GVNameStyle"))
            Else
                'Style inner cell
                recipRow.Cells(k).BackColor = IIf(rowStatus = "Complete", Drawing.ColorTranslator.FromHtml("#958e5b"), IIf(rowStatus = "Partial", Drawing.ColorTranslator.FromHtml("#c9b370"), Nothing))
            End If
        Next

        cat_updatePnl.Update()

    End Sub

    Private Sub bindGiftsGV(ByVal recipID As String)

        Dim inner_gv As GridView
        Dim checkmark_img As Image
        Dim outerCbx As CheckBox
        Dim catID As Integer
        Dim rowStatus As String
        Dim recipRow As GridViewRow

        'Bind just this inner GridView
        For i As Integer = 0 To cat_repeater.Items.Count - 1

            Dim gv As GridView = CType(cat_repeater.Items(i).FindControl("recipients_gv"), GridView)

            For j As Integer = 0 To gv.Rows.Count - 1

                If gv.DataKeys(j).Values("ID") = recipID Then

                    inner_gv = CType(gv.Rows(j).FindControl("gifts_gv"), GridView)
                    recipRow = gv.Rows(j)
                    outerCbx = CType(gv.Rows(j).FindControl("recipientComplete_cbx"), CheckBox)
                    catID = CType(cat_repeater.Items(i).FindControl("id_hField"), HiddenField).Value
                    checkmark_img = CType(cat_repeater.Items(i).FindControl("checkmark_img"), Image)

                    oCmd.Connection = oConn
                    oCmd.CommandType = CommandType.Text

                    oCmd.Parameters.Clear()
                    gifts_dTbl.Clear()

                    strSQL = "SELECT * FROM Gifts WHERE RecipientID = @RecipientID ORDER BY Description"

                    oParam = New SqlParameter
                    oParam.ParameterName = "RecipientID"
                    oParam.SqlDbType = SqlDbType.Int
                    oParam.Value = recipID
                    oCmd.Parameters.Add(oParam)

                    oCmd.CommandText = strSQL
                    oDA.SelectCommand = oCmd

                    oDA.Fill(gifts_dTbl)

                    inner_gv.DataSource = gifts_dTbl
                    inner_gv.DataBind()

                    For k As Integer = 0 To gifts_dTbl.Rows.Count - 1
                        Dim inner_cbx As CheckBox = CType(inner_gv.Rows(k).FindControl("giftComplete_cbx"), CheckBox)
                        Session("Gift_" & inner_cbx.ClientID & "_State") = inner_cbx.Checked
                        Session(inner_gv.ClientID & "_AllChecked") = allGiftCheckboxesChecked(inner_gv)
                    Next

                    Exit For

                End If

            Next

        Next

        checkmark_img.Visible = getCategoryCompleteStatus(catID)

        rowStatus = getRecipientCompleteStatus(recipID)

        outerCbx.Checked = (rowStatus = "Complete")

        'If checking the inner checkbox completes all gifts for a recipient (rowStatus), style the whole outer row (since the outer GridView's databound
        'event isn't called, this needs to be done separately)
        For k As Integer = 0 To recipRow.Cells.Count - 1
            If k <> 1 Then
                recipRow.Cells(k).CssClass = IIf(rowStatus = "Complete", "GVItemCompleteStyle", IIf(rowStatus = "Partial", "GVItemPartialStyle", "GVNameStyle"))
            Else
                'Style inner cell
                recipRow.Cells(k).BackColor = IIf(rowStatus = "Complete", Drawing.ColorTranslator.FromHtml("#958e5b"), IIf(rowStatus = "Partial", Drawing.ColorTranslator.FromHtml("#c9b370"), Nothing))
            End If
        Next

        cat_updatePnl.Update()

    End Sub

    Protected Sub nameSortClick(ByVal sender As Object, ByVal e As System.EventArgs)
        If Session("NameSortDir") = SortDirection.Descending Then
            Session("NameSortDir") = SortDirection.Ascending
            bindRecipsGV(sender, "nameSort_lBtn", "LastName ASC, FirstName ASC")
        ElseIf Session("NameSortDir") = SortDirection.Ascending Then
            Session("NameSortDir") = SortDirection.Descending
            bindRecipsGV(sender, "nameSort_lBtn", "LastName DESC, FirstName DESC")
        End If
    End Sub

    Protected Sub giftCompleteSortClick(ByVal sender As Object, ByVal e As System.EventArgs)
        If Session("GiftCompleteSortDir") = SortDirection.Descending Then
            Session("GiftCompleteSortDir") = SortDirection.Ascending
            bindGiftsGV(sender, "giftCompleteSort_lBtn", "Complete ASC")
        ElseIf Session("GiftCompleteSortDir") = SortDirection.Ascending Then
            Session("GiftCompleteSortDir") = SortDirection.Descending
            bindGiftsGV(sender, "giftCompleteSort_lBtn", "Complete DESC")
        End If
    End Sub

    Protected Sub descSortClick(ByVal sender As Object, ByVal e As System.EventArgs)
        If Session("DescSortDir") = SortDirection.Descending Then
            Session("DescSortDir") = SortDirection.Ascending
            bindGiftsGV(sender, "descSort_lBtn", "Description ASC")
        ElseIf Session("DescSortDir") = SortDirection.Ascending Then
            Session("DescSortDir") = SortDirection.Descending
            bindGiftsGV(sender, "descSort_lBtn", "Description DESC")
        End If
    End Sub

    Protected Sub gift_fv_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles gift_fv.DataBound
        If gift_fv.CurrentMode <> FormViewMode.ReadOnly Then
            ToolkitScriptManager1.SetFocus(CType(gift_fv.FindControl("desc_txt"), TextBox))
        End If
    End Sub

    Protected Sub gift_fv_ItemCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewCommandEventArgs) Handles gift_fv.ItemCommand
        If e.CommandName = "Cancel" Then
            fv_mpExt.Hide()
        End If
    End Sub


    Protected Sub gift_fv_ItemDeleted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewDeletedEventArgs) Handles gift_fv.ItemDeleted
        fv_mpExt.Hide()
        bindRecipsGV()
    End Sub

    Protected Sub gift_fv_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewInsertedEventArgs) Handles gift_fv.ItemInserted
        fv_mpExt.Hide()
        bindRecipsGV()
    End Sub


    Protected Sub gift_fv_ItemUpdated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewUpdatedEventArgs) Handles gift_fv.ItemUpdated
        fv_mpExt.Hide()
        bindGiftsGV(Session("SelectedGiftRecipient"))
    End Sub

    Protected Sub gift_sds_Inserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.SqlDataSourceStatusEventArgs) Handles gift_sds.Inserted
        Session("SelectedGift") = CType(e.Command.Parameters("@NewID").Value, Integer)
    End Sub


    Protected Sub recipient_fv_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles recipient_fv.DataBound
        If recipient_fv.CurrentMode <> FormViewMode.ReadOnly Then
            ToolkitScriptManager1.SetFocus(CType(recipient_fv.FindControl("fName_txt"), TextBox))
        End If
    End Sub

    Protected Sub recipient_fv_ItemCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewCommandEventArgs) Handles recipient_fv.ItemCommand
        If e.CommandName = "Cancel" Then
            fv_mpExt.Hide()
        End If
    End Sub

    Protected Sub recipient_fv_ItemDeleted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewDeletedEventArgs) Handles recipient_fv.ItemDeleted
        fv_mpExt.Hide()
        bindCategories()
    End Sub

    Protected Sub recipient_fv_ItemDeleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewDeleteEventArgs) Handles recipient_fv.ItemDeleting
        Try
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            strSQL = "DELETE FROM Gifts WHERE RecipientID = @RecipientID"

            oCmd.Parameters.Clear()

            oParam = New SqlParameter()
            oParam.ParameterName = "RecipientID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = Session("SelectedRecipient")
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = strSQL

            oCmd.Connection.Open()
            oCmd.ExecuteScalar()

        Catch ex As Exception
            Throw ex
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If

            oCmd.Dispose()
        End Try
    End Sub


    Protected Sub recipient_fv_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewInsertedEventArgs) Handles recipient_fv.ItemInserted
        fv_mpExt.Hide()
        bindCategories()
    End Sub

    Protected Sub recipient_fv_ItemUpdated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewUpdatedEventArgs) Handles recipient_fv.ItemUpdated
        fv_mpExt.Hide()
        'A recipient could potentially be placed in a different category, so I want the change reflected on the category level
        bindCategories()
    End Sub


    Protected Sub recipient_sds_Inserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.SqlDataSourceStatusEventArgs) Handles recipient_sds.Inserted
        Session("SelectedRecipient") = CType(e.Command.Parameters("@NewID").Value, Integer)
    End Sub

    Protected Sub category_fv_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles category_fv.DataBound
        If category_fv.CurrentMode <> FormViewMode.ReadOnly Then
            ToolkitScriptManager1.SetFocus(CType(category_fv.FindControl("title_txt"), TextBox))
        End If
    End Sub


    Protected Sub category_fv_ItemCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewCommandEventArgs) Handles category_fv.ItemCommand
        If e.CommandName = "Cancel" Then
            fv_mpExt.Hide()
        End If
    End Sub

    Protected Sub category_fv_ItemDeleted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewDeletedEventArgs) Handles category_fv.ItemDeleted
        fv_mpExt.Hide()
        bindCategories()
    End Sub

    Protected Sub category_fv_ItemDeleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewDeleteEventArgs) Handles category_fv.ItemDeleting

        Try
            'Get all recipient IDs for the category being deleted, delete all gifts for those recipients, 
            'delete all recipients for that category
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            strSQL = "SELECT ID FROM Recipients WHERE CategoryID = @CategoryID"

            oCmd.Parameters.Clear()
            oDTbl.Clear()

            oParam = New SqlParameter
            oParam.ParameterName = "CategoryID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = Session("SelectedCategory")
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = strSQL
            oDA.SelectCommand = oCmd

            oDA.Fill(oDTbl)

            strSQL = "DELETE FROM Gifts WHERE RecipientID = @RecipientID"

            For i As Integer = 0 To oDTbl.Rows.Count - 1

                oCmd.Parameters.Clear()

                If giftsForRecipient(oDTbl.Rows(i)("ID")) > 0 Then

                    oParam = New SqlParameter()
                    oParam.ParameterName = "RecipientID"
                    oParam.SqlDbType = SqlDbType.Int
                    oParam.Value = oDTbl.Rows(i)("ID")
                    oCmd.Parameters.Add(oParam)

                    oCmd.CommandText = strSQL
                    oCmd.Connection.Open()

                    oCmd.ExecuteScalar()
                    oCmd.Connection.Close()

                End If


            Next

            strSQL = "DELETE FROM Recipients WHERE CategoryID = @CategoryID"

            oCmd.Parameters.Clear()

            oParam = New SqlParameter
            oParam.ParameterName = "CategoryID"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = Session("SelectedCategory")
            oCmd.Parameters.Add(oParam)

            oCmd.CommandText = strSQL

            oCmd.Connection.Open()
            oCmd.ExecuteScalar()
            oCmd.Connection.Close()
        Catch ex As Exception
            Throw ex
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If

            oCmd.Dispose()
        End Try
    End Sub


    Protected Sub category_fv_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewInsertedEventArgs) Handles category_fv.ItemInserted
        fv_mpExt.Hide()
        bindCategories()
    End Sub

    Protected Sub category_fv_ItemUpdated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewUpdatedEventArgs) Handles category_fv.ItemUpdated
        fv_mpExt.Hide()
        bindCategories()
    End Sub



    Protected Sub category_sds_Inserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.SqlDataSourceStatusEventArgs) Handles category_sds.Inserted
        Session("SelectedCategory") = CType(e.Command.Parameters("@NewID").Value, Integer)
    End Sub

    Protected Sub outerCancelClick(ByVal sender As Object, ByVal e As System.EventArgs)
        fv_mpExt.Hide()
    End Sub

    Protected Sub retainGiftStates_cbx_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles retainGiftStates_cbx.CheckedChanged
        'I am setting this here to accommodate for a scenario in which gift checkboxes are checked while retain is not checked, then
        'retain is checked, and a recipient is unchecked

        retainCbx_pnl.CssClass = IIf(retainGiftStates_cbx.Checked, "RetainActiveStyle", "")

        For i As Integer = 0 To cat_repeater.Items.Count - 1
            Dim gv As GridView = CType(cat_repeater.Items(i).FindControl("recipients_gv"), GridView)

            For j As Integer = 0 To gv.Rows.Count - 1

                Dim inner_gv As GridView = CType(gv.Rows(j).FindControl("gifts_gv"), GridView)

                For k As Integer = 0 To inner_gv.Rows.Count - 1
                    Dim cbx As CheckBox = CType(inner_gv.Rows(k).FindControl("giftComplete_cbx"), CheckBox)
                    Session("Gift_" & cbx.ClientID & "_State") = cbx.Checked
                    Session(inner_gv.ClientID & "_AllChecked") = allGiftCheckboxesChecked(inner_gv)
                Next

            Next

        Next

    End Sub


End Class

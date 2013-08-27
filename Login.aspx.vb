
Partial Class Login
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        SetFocus(CType(login2.FindControl("UserName"), TextBox))
    End Sub
End Class

<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Gift Lister: A Christmas Gift Recipient Manager - Log In</title>
    
    <link href="favicon.ico" rel="icon" type="image/x-icon" />
    <link href="style.css" rel="stylesheet" type="text/css" />
    
</head>
<body>
    <form id="form1" runat="server">
    
        <h1>Gift Lister</h1>
        <h2>A Christmas Gift Recipient Manager</h2>
        
        <div id="outer_div">
            <h3>Log In</h3>
            <table id="blank_outer_table">
                <tr>
                    <td>
                        
                        <asp:Login ID="login2" runat="server" DisplayRememberMe="false" BackColor="#e1da96" Width="375px" BorderColor="#d1ca8a" BorderWidth="2px" BorderStyle="Solid">
                            <TextBoxStyle CssClass="InputStyle" />
                            <LoginButtonStyle CssClass="ButtonStyle" />
                            <InstructionTextStyle Font-Size="11px" />
                            <LabelStyle Font-Bold="true" ForeColor="#393726" HorizontalAlign="Left" Font-Size="12px" />
                            <TitleTextStyle font-bold="true" backcolor="#c3bd81" ForeColor="#393726" Font-Size="12px"  />
                        </asp:Login>
                        <br />
                        <asp:PasswordRecovery ID="passwordRecovery2" runat="server" BackColor="#e1da96" Width="375px" BorderColor="#d1ca8a" BorderWidth="2px" BorderStyle="Solid">
                            <TextBoxStyle CssClass="InputStyle" />
                            <SubmitButtonStyle CssClass="ButtonStyle" /> 
                            <InstructionTextStyle Font-Size="11px" />
                            <LabelStyle Font-Bold="true" ForeColor="#393726" HorizontalAlign="Left" Font-Size="12px" />
                            <TitleTextStyle font-bold="true" backcolor="#c3bd81" ForeColor="#393726" Font-Size="12px" />
                        </asp:PasswordRecovery>

                    </td>
                </tr>
            </table>
            
        </div>
        
        <div class="Footer">
            Copyright &copy; 2011, <a href="mailto:maggy@zogglet.com?subject=About your awesome Christmas Gift Lister">Maggy Maffia</a>
        </div>
        
    </form>
</body>
</html>

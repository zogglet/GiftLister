<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Images.aspx.vb" Inherits="Images" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>Gift Lister: A Christmas Gift Recipient Manager</title>
    
    <link href="favicon.ico" rel="icon" type="image/x-icon" />
    <link href="style.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
    
        <%--Required for use of AJAX Control Toolkit --%>
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" EnablePartialRendering="true" />
    
        <div class="LoginStatusStyle">
                <asp:LoginView ID="loginView" runat="server">
                    <AnonymousTemplate>
                        <i>Currently logged out</i>
                        <br />
                        <asp:LoginStatus ID="loginStatus" runat="server" LoginText="[Log In]" LogoutText="[Log Out]" />
                    </AnonymousTemplate>
                    <LoggedInTemplate>
                        <asp:LoginName ID="loginName" runat="server" FormatString="Greetings, <b>{0}</b>!" />
                        <br />
                        <asp:LoginStatus ID="loginStatus" runat="server" LoginText="[Log In]" LogoutText="[Log Out]" />
                    </LoggedInTemplate>
                </asp:LoginView>
            </div>
            
        <h1>Gift Lister</h1>
        <h2>A Christmas Gift Recipient Manager</h2>
    
        <div id="outer_div">
        
            <table id="outer_table">
                
                <tr>
                    <td>
                        
                        <asp:UpdatePanel ID="images_updatePnl" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                            
                                <asp:Label ID="gift_lbl" runat="server" Text='<%#formatGiftHeader() %>' />
                                <span class="Divider">&nbsp;</span>
                                <br />
                        
                                <table class="options_area">
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="images_ddl" runat="server" DataTextField="ImageTitle" DataValueField="ID" AutoPostBack="true" CssClass="InputStyle"
                                                OnSelectedIndexChanged="imagesSelectedIndexChanged" />
                                        </td>
                                        <td>
                                            <asp:LinkButton ID="viewAll_lBtn" runat="server" Text="[View All Images]" CausesValidation="false" OnClick="viewAllClick" />
                                        </td>
                                        <td>
                                            <asp:Literal ID="prompt_lit" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                
                                <%--Reffing this article: http://www.4guysfromrolla.com/articles/120606-1.aspx and BLOBsInDB code in /Mine--%>
                                <asp:FormView ID="images_fv" runat="server" DataKeyNames="ID" DataSourceID="image_sds" Width="100%">
                                    
                                    <HeaderTemplate>
                                        <table class="FormViewTbl">
                                    </HeaderTemplate>
                                    
                                    <EditItemTemplate>
                                            <tr>
                                                <td>
                                                    Image Title:
                                                    &nbsp;<asp:TextBox ID="imgTitle_txt" runat="server" Text='<%#Bind("ImageTitle") %>' Width="150px" CssClass="InputStyle" />
                                                    
                                                    <asp:RequiredFieldValidator ID="imgTitle_rVal" runat="server" ControlToValidate="imgTitle_txt" ErrorMessage="Image title is required." Display="None" />
                                                    <asp:ValidatorCalloutExtender ID="imgTitle_vcExt" runat="server" TargetControlID="imgTitle_rVal" WarningIconImageUrl="warningIcon.png"
                                                          CloseImageUrl="closeIcon.png" CssClass="ValidatorCalloutStyle" />
                                                </td>
                                                <td>
                                                    New Image: 
                                                    <asp:HiddenField ID="image_hField" runat="server" Value='<%#Bind("ImageData") %>' />
                                                    &nbsp;<asp:FileUpload ID="image_uploader" runat="server" CssClass="InputStyle" Width="150px" />
                                                    
                                                    <%--So I can bind to e.oldValues--%>
                                                    <asp:HiddenField ID="mimeType_hField" runat="server" Value='<%#Bind("MIMEType") %>' />
                                                   
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" style="text-align: center;">
                                                    <br />
                                                    <asp:Panel ID="previewImg_pnl" runat="server"></asp:Panel>
                                                    <%--<asp:Image ID="preview_img" runat="server" ImageUrl='<%#Eval("ID", "DisplayImage.aspx?ID={0}") %>' BorderColor="#9a9566" BorderWidth="1px" BorderStyle="Solid" />--%>
                                                    <br /><asp:Literal ID="imageStats_lit" runat="server" Text='<%#formatImageStats() %>' />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <span class="ModalDivider">&nbsp;</span>
                                                    <br />
                                                    
                                                    <asp:Button ID="update_btn" runat="server" CausesValidation="true" CommandName="Update" Text="Update" CssClass="ButtonStyle" />
                                                    &nbsp;&nbsp;
                                                    <asp:Button ID="cancel_btn" runat="server" CausesValidation="false" CommandName="Cancel" Text="Cancel" CssClass="ButtonStyle" />
                                                </td>
                                            </tr>
                                    </EditItemTemplate>
                                    
                                    <InsertItemTemplate>
                                            <tr>
                                                <td>
                                                    Image Title:
                                                    &nbsp;<asp:TextBox ID="imgTitle_txt" runat="server" Text='<%#Bind("ImageTitle") %>' Width="150px" CssClass="InputStyle" />
                                                    
                                                    <asp:RequiredFieldValidator ID="imgTitle_rVal" runat="server" ControlToValidate="imgTitle_txt" ErrorMessage="Image title is required." Display="None" />
                                                    <asp:ValidatorCalloutExtender ID="imgTitle_vcExt" runat="server" TargetControlID="imgTitle_rVal" WarningIconImageUrl="warningIcon.png"
                                                          CloseImageUrl="closeIcon.png" CssClass="ValidatorCalloutStyle" />
                                                </td>
                                                <td>
                                                    Image: 
                                                    &nbsp;<asp:FileUpload ID="image_uploader" runat="server" CssClass="InputStyle" Width="150px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <span class="ModalDivider">&nbsp;</span>
                                                    <br />
                                                    
                                                    <asp:Button ID="insert_btn" runat="server" CausesValidation="true" CommandName="Insert" Text="Add" CssClass="ButtonStyle" />
                                                    &nbsp;&nbsp;
                                                    <asp:Button ID="cancel_btn" runat="server" CausesValidation="false" CommandName="Cancel" Text="Cancel" CssClass="ButtonStyle" />
                                                </td>
                                            </tr>
                                    </InsertItemTemplate>
                                    
                                    <ItemTemplate>
                                            <tr>
                                                <td>
                                                    Image Title:
                                                    &nbsp;<asp:Label ID="imageData_lbl" runat="server" Text='<%#Bind("ImageTitle") %>' CssClass="StandAloneFormViewLbl" />
                                                </td>
                                                <td>
                                                    MIME Type: 
                                                    &nbsp;<asp:Label ID="mimeType_lbl" runat="server" Text='<%#Bind("MIMEType") %>' CssClass="StandAloneFormViewLbl" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" style="text-align: center;">
                                                    <br />
                                                    <asp:Panel ID="previewImg_pnl" runat="server"></asp:Panel>
                                                    <br /><asp:Literal ID="imageStats_lit" runat="server" Text='<%#formatImageStats() %>' />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <span class="ModalDivider">&nbsp;</span>
                                                    <br />
                                                    
                                                    <asp:Button ID="back_btn" runat="server" CausesValidation="false" CommandName="Cancel" Text="&laquo; Back" CssClass="ButtonStyle" />
                                                    &nbsp;&nbsp;
                                                    <asp:Button ID="edit_btn" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" CssClass="ButtonStyle" />
                                                    &nbsp;&nbsp;
                                                    <asp:Button ID="delete_btn" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"
                                                        OnClientClick="return confirm('Are you sure you want to delete this image?');" CssClass="ButtonStyle" />
                                                    
                                                </td>
                                            </tr>
                                    </ItemTemplate>
                                    
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                    
                                </asp:FormView>
                                
                                <%--To list all images--%>
                                <asp:Panel ID="viewImages_pnl" runat="server" Visible="false">

                                    <asp:DataList ID="images_dList" runat="server" RepeatColumns="4" RepeatDirection="Horizontal" HorizontalAlign="Center" ItemStyle-VerticalAlign="Top">
                                        <ItemTemplate>
                                            <%--Image tags are created here.--%>
                                            <asp:Panel ID="listImg_pnl" runat="server" CssClass="ListImagePnlStyle"></asp:Panel>
                                            
                                            <asp:HoverMenuExtender ID="imgStats_hmExt" runat="server" TargetControlID="listImg_pnl" PopupControlID="imgStats_pnl"
                                                    Popupposition="Right" HoverDelay="0" />
                                            <asp:Panel ID="imgStats_pnl" runat="server" CssClass="ToolTipStyle">
                                                <asp:Literal ID="imgStats_lit" runat="server" Text='<%#formatToolTipStats(eval("ID"), eval("ImageTitle")) %>' />
                                            </asp:Panel>
                                            
                                        </ItemTemplate>
                                    </asp:DataList>
                                    
                                </asp:Panel>
                                
                                <br />
                                <span style="text-align:right;display:block;">
                                    <asp:Button ID="outerCancel_btn" runat="server" CssClass="ButtonStyle" Text="&laquo; Back" OnClick="outerCancelClick" />
                                </span>
                                
                            </ContentTemplate>
                            <Triggers>
                                
                            </Triggers>
                        </asp:UpdatePanel>
                        
                    </td>
                </tr>
                
            </table>
        
            <asp:SqlDataSource ID="image_sds" runat="server" ConnectionString="<%$ ConnectionStrings:GiftListConnectionString %>"
                SelectCommand="SELECT Images.ID, ImageTitle, MIMEType, ImageData, GiftID FROM Images INNER JOIN Gifts ON Images.GiftID = Gifts.ID WHERE Images.ID = @ID"
                InsertCommand="INSERT INTO Images (ImageTitle, MIMEType, ImageData, GiftID) VALUES (@ImageTitle, @MIMEType, @ImageData, @GiftID);SET @NewID = SCOPE_IDENTITY()"
                UpdateCommand="UPDATE Images SET ImageTitle = @ImageTitle, MIMEType = @MimeType, ImageData = @ImageData WHERE ID = @ID"
                DeleteCommand="DELETE FROM Images WHERE ID = @ID"
                >
            
                <SelectParameters>
                    <asp:SessionParameter Name="ID" SessionField="SelectedImage" Type="Int32" />
                </SelectParameters>
                
                <DeleteParameters>
                    <asp:Parameter Name="ID" Type="Int32" />
                </DeleteParameters>
                
                <UpdateParameters>
                    <asp:Parameter Name="ID" Type="Int32" />
                    <asp:Parameter Name="ImageTitle" Type="String" />
                    <asp:Parameter Name="MIMEType" Type="String" />
                    <asp:Parameter Name="ImageData" />
                </UpdateParameters>
                
                <InsertParameters>
                    <asp:Parameter Name="ImageTitle" Type="String" />
                    <asp:Parameter Name="MIMEType" Type="String" />
                    <asp:Parameter Name="ImageData" />
                    <asp:SessionParameter Name="GiftID" SessionField="SelectedImagesGift" type="Int32" />
                    <asp:Parameter Name="NewID" Type="Int32" Size="4" Direction="Output" />
                </InsertParameters>
                
            </asp:SqlDataSource>
        
        </div>
        
        <div class="Footer">
            Copyright &copy; 2011, <a href="mailto:maggy@zogglet.com?subject=About your awesome Christmas Gift Lister">Maggy Maffia</a>
        </div>
    </form>
</body>
</html>

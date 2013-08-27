<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default_imgFvInPopup.aspx.vb" Inherits="_Default" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Gift Lister: A Christmas Gift Recipient Manager</title>
    
    <link href="favicon.ico" rel="icon" type="image/x-icon" />
    <link href="style.css" rel="stylesheet" type="text/css" />
    
</head>

<body>

        <script type="text/javascript" language="javascript">

            var postbackHappened = false;

            //Prevents Timer from snapping to the top of the page on each tick
            function scrollTo() {
                return;
            }

            //Hackish way of causing a postback from the main UpdatePanel, thus properly loading all dynamically-generated elements
            function pageLoad() {
                if (postbackHappened) {
                    return false;
                } else {
                    postbackHappened = true;

                    var btn = $get('<%= postback_btn.ClientID %>');
                    btn.click();
                }
            }

            window.onload = pageLoad;

    </script>

    <form id="form1" runat="server" name="form1" enctype="multipart/form-data">
    
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
    
        <%--General ModalPopup for FormViews--%>
        <asp:ModalPopupExtender ID="fv_mpExt" runat="server" TargetControlID="dummy" PopupControlID="fv_pnl" />
        
        <input type="button" id="dummy" runat="server" style="display: none;" />
        
        <asp:Panel ID="fv_pnl" runat="server" CssClass="ModalStyle" Width="375px">
        
            <asp:UpdatePanel ID="fv_updatePnl" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                <ContentTemplate>
                
                    <%--Category FormView--%>
                    <asp:FormView ID="category_fv" runat="server" DataKeyNames="ID" DataSourceID="category_sds" Width="100%">
                        
                        <HeaderTemplate>
                            <table class="FormViewTbl">
                                <tr>
                                    <td>
                                        <h3>Add/Edit Category</h3>
                                        <span class="ModalDivider">&nbsp;</span>
                                        <br />
                                    </td>
                                </tr>
                        </HeaderTemplate>
                        
                        <EditItemTemplate>
                                <tr>
                                    <td>
                                        Category Title:
                                        &nbsp;<asp:TextBox ID="title_txt" runat="server" Text='<%#Bind("Title") %>' CssClass="InputStyle" />
                                        
                                        <asp:RequiredFieldValidator ID="title_rVal" runat="server" ValidationGroup="category_vGroup" ControlToValidate="title_txt" ErrorMessage="Category title is required." Display="None" />
                                        <asp:ValidatorCalloutExtender ID="title_vcExt" runat="server" TargetControlID="title_rVal" WarningIconImageUrl="warningIcon.png"
                                              CloseImageUrl="closeIcon.png" CssClass="ValidatorCalloutStyle" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <span class="ModalDivider">&nbsp;</span>
                                        <br />
                                        
                                        <asp:Button ID="update_btn" runat="server" CausesValidation="true" ValidationGroup="category_vGroup" CommandName="Update" Text="Update" CssClass="ButtonStyle" />
                                        &nbsp;&nbsp;
                                        <asp:Button ID="cancel_btn" runat="server" CausesValidation="false" CommandName="Cancel" Text="Cancel" CssClass="ButtonStyle" />
                                    </td>
                                </tr>
                        </EditItemTemplate>
                        
                        <InsertItemTemplate>
                                <tr>
                                    <td>
                                        Category Title:
                                        &nbsp;<asp:TextBox ID="title_txt" runat="server" Text='<%#Bind("Title") %>' CssClass="InputStyle" />
                                        
                                        <asp:RequiredFieldValidator ID="title_rVal" runat="server" ValidationGroup="category_vGroup" ControlToValidate="title_txt" ErrorMessage="Category title is required." Display="None" />
                                        <asp:ValidatorCalloutExtender ID="title_vcExt" runat="server" TargetControlID="title_rVal" WarningIconImageUrl="warningIcon.png"
                                              CloseImageUrl="closeIcon.png" CssClass="ValidatorCalloutStyle" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <span class="ModalDivider">&nbsp;</span>
                                        <br />
                                        
                                        <asp:Button ID="insert_btn" runat="server" CausesValidation="true" ValidationGroup="category_vGroup" CommandName="Insert" Text="Add" CssClass="ButtonStyle" />
                                        &nbsp;&nbsp;
                                        <asp:Button ID="cancel_btn" runat="server" CausesValidation="false" CommandName="Cancel" Text="Cancel" CssClass="ButtonStyle" />
                                    </td>
                                </tr>
                        </InsertItemTemplate>
                        
                        <ItemTemplate>
                                <tr>
                                    <td>
                                        Category Title: 
                                        &nbsp;<asp:Label ID="title_lbl" runat="server" Text='<%#Bind("Title") %>' CssClass="FormViewLbl" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span class="ModalDivider">&nbsp;</span>
                                        <br />
                                        <asp:Button ID="edit_btn" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" CssClass="ButtonStyle" />
                                        &nbsp;&nbsp;
                                        <asp:Button ID="delete_btn" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"
                                            OnClientClick="return confirm('Deleting this category will delete all recipients under this category and their respective gifts. \nAre you sure you want to delete this category?');" CssClass="ButtonStyle" />
                                        &nbsp;&nbsp;
                                        <asp:Button ID="cancel_btn" runat="server" CausesValidation="false" CommandName="Cancel" Text="Cancel" CssClass="ButtonStyle" />  
                                    </td>
                                </tr>
                        </ItemTemplate>
                        
                    </asp:FormView>
                    <%--/Category FormView--%>
                    
                    <%--Recipient FormView--%>
                    <asp:FormView ID="recipient_fv" runat="server" DataKeyNames="ID" DataSourceID="recipient_sds" Width="100%">
                        
                        <HeaderTemplate>
                            <table class="FormViewTbl">
                                <tr>
                                    <td colspan="2">
                                        <asp:Label ID="recip_lbl" runat="server" Text='<%#formatCatHeader()%>' />
                                        <span class="ModalDivider">&nbsp;</span>
                                        <br />
                                    </td>
                                </tr>
                        </HeaderTemplate>
                        
                        <EditItemTemplate>
                            <tr>
                                <td>
                                    First Name:
                                    &nbsp;<asp:TextBox ID="fName_txt" runat="server" Text='<%#Bind("FirstName") %>' CssClass="InputStyle" />
                                    
                                    <asp:RequiredFieldValidator ID="fName_rVal" runat="server" ValidationGroup="recipient_vGroup" ControlToValidate="fName_txt" ErrorMessage="First name is required." Display="None" />
                                    <asp:ValidatorCalloutExtender ID="fName_vcExt" runat="server" TargetControlID="fName_rVal" WarningIconImageUrl="warningIcon.png"
                                          CloseImageUrl="closeIcon.png" CssClass="ValidatorCalloutStyle" />
                                </td>
                                <td>
                                    Last Name:
                                    &nbsp;<asp:TextBox ID="lName_txt" runat="server" Text='<%#Bind("LastName") %>' CssClass="InputStyle" />
                                </td>
                            </tr>
                            
                            <tr>
                                <td>
                                    Category: 
                                    &nbsp;<asp:DropDownList ID="category_ddl" runat="server" DataSourceID="categories_sds" DataTextField="Title" DataValueField="ID" SelectedValue='<%#Bind("CategoryID") %>' CssClass="InputStyle" />
                                    
                                    <asp:SqlDataSource ID="categories_sds" runat="server" ConnectionString="<%$ ConnectionStrings:GiftListConnectionString %>" 
                                        SelectCommand="SELECT ID, Title FROM Categories UNION SELECT -1 AS ID, '-- Select Category --' AS Title ORDER BY ID" />
                                        
                                    <asp:CompareValidator ID="category_cVal" runat="server" ValidationGroup="recipient_vGroup" Operator="NotEqual" ValueToCompare="-1" Display="None" 
                                        ControlToValidate="category_ddl" ErrorMessage="Category is required." />
                                    <asp:ValidatorCalloutExtender ID="category_vcExt" runat="server" TargetControlID="category_cVal" WarningIconImageUrl="warningIcon.png"
                                        CloseImageUrl="closeIcon.png" CssClass="ValidatorCalloutStyle" />
                                    
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <span class="ModalDivider">&nbsp;</span>
                                    <br />
                                    
                                    <asp:Button ID="update_btn" runat="server" CausesValidation="true" ValidationGroup="recipient_vGroup" CommandName="Update" Text="Update" CssClass="ButtonStyle" />
                                    &nbsp;&nbsp;
                                    <asp:Button ID="cancel_btn" runat="server" CausesValidation="false" CommandName="Cancel" Text="Cancel" CssClass="ButtonStyle" />
                                </td>
                            </tr>
                        </EditItemTemplate>
                        
                        <InsertItemTemplate>
                            <tr>
                                <td>
                                    First Name:
                                    &nbsp;<asp:TextBox ID="fName_txt" runat="server" Text='<%#Bind("FirstName") %>' CssClass="InputStyle" />
                                    
                                    <asp:RequiredFieldValidator ID="fName_rVal" runat="server" ValidationGroup="recipient_vGroup" ControlToValidate="fName_txt" ErrorMessage="First name is required." Display="None" />
                                    <asp:ValidatorCalloutExtender ID="catTitle_vcExt" runat="server" TargetControlID="fName_rVal" WarningIconImageUrl="warningIcon.png"
                                          CloseImageUrl="closeIcon.png" CssClass="ValidatorCalloutStyle" />
                                </td>
                                <td>
                                    Last Name:
                                    &nbsp;<asp:TextBox ID="lName_txt" runat="server" Text='<%#Bind("LastName") %>' CssClass="InputStyle" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <span class="ModalDivider">&nbsp;</span>
                                    <br />
                                    
                                    <asp:Button ID="insert_btn" runat="server" CausesValidation="true" ValidationGroup="recipient_vGroup" CommandName="Insert" Text="Add" CssClass="ButtonStyle" />
                                    &nbsp;&nbsp;
                                    <asp:Button ID="cancel_btn" runat="server" CausesValidation="false" CommandName="Cancel" Text="Cancel" CssClass="ButtonStyle" />
                                </td>
                            </tr>
                        </InsertItemTemplate>
                        
                        <ItemTemplate>
                            <tr>
                                <td>
                                    First Name:
                                    &nbsp;<asp:Label ID="fName_lbl" runat="server" Text='<%#Bind("FirstName") %>' CssClass="FormViewLbl" />
                                </td>
                                <td>
                                    Last Name:
                                    &nbsp;<asp:Label ID="lName_lbl" runat="server" Text='<%#formatNullField(eval("LastName")) %>' CssClass="FormViewLbl" />
                                </td>
                            </tr>
                            
                            <tr>
                                <td>
                                    Category: 
                                    &nbsp;<asp:Label ID="category_lbl" runat="server" Text='<%#Bind("Title") %>' CssClass="FormViewLbl" />
                                    
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                
                                    <span class="ModalDivider">&nbsp;</span>
                                    <br />
                                    <asp:Button ID="edit_btn" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" CssClass="ButtonStyle" />
                                    &nbsp;&nbsp;
                                    <asp:Button ID="delete_btn" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"
                                        OnClientClick="return confirm('Deleting this recipient will delete all gifts assigned to this recipient. \nAre you sure you want to delete this recipient?');" CssClass="ButtonStyle" />
                                    &nbsp;&nbsp;
                                    <asp:Button ID="cancel_btn" runat="server" CausesValidation="false" CommandName="Cancel" Text="Cancel" CssClass="ButtonStyle" /> 
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:FormView>

                    <%--/Recipient Formview--%>
                    
                
                    <%--Gift FormView--%>
                    <asp:FormView ID="gift_fv" runat="server" DataKeyNames="ID" DataSourceID="gift_sds" Width="100%">
                        
                        <HeaderTemplate>
                            <table class="FormViewTbl">
                                <tr>
                                    <td colspan="2">
                                        <asp:Label ID="recip_lbl" runat="server" Text='<%#formatRecipHeader()%>' />
                                        <span class="ModalDivider">&nbsp;</span>
                                        <br />
                                    </td>
                                </tr>
                        </HeaderTemplate>
                        
                        <EditItemTemplate>
                                <tr>
                                    <td>
                                        Gift description:
                                        &nbsp;<asp:TextBox ID="desc_txt" runat="server" Text='<%#Bind("Description") %>' Width="150px" CssClass="InputStyle" />
                                        
                                        <asp:RequiredFieldValidator ID="desc_rVal" runat="server" ValidationGroup="gift_vGroup" ControlToValidate="desc_txt" ErrorMessage="Description is required." Display="None" />
                                        <asp:ValidatorCalloutExtender ID="desc_vcExt" runat="server" TargetControlID="desc_rVal" WarningIconImageUrl="warningIcon.png"
                                              CloseImageUrl="closeIcon.png" CssClass="ValidatorCalloutStyle" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="complete_cbx" runat="server" Checked='<%#Bind("Complete") %>' Text="Complete" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        Notes:
                                        <br />
                                        <asp:TextBox ID="notes_txt" runat="server" Text='<%#Bind("Notes") %>' TextMode="MultiLine" Wrap="true" Width="360px" Height="80px" CssClass="InputStyle" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <span class="ModalDivider">&nbsp;</span>
                                        <br />
                                        
                                        <asp:Button ID="update_btn" runat="server" CausesValidation="true" ValidationGroup="gift_vGroup" CommandName="Update" Text="Update" CssClass="ButtonStyle" />
                                        &nbsp;&nbsp;
                                        <asp:Button ID="cancel_btn" runat="server" CausesValidation="false" CommandName="Cancel" Text="Cancel" CssClass="ButtonStyle" />
                                    </td>
                                </tr>
                        </EditItemTemplate>
                        
                        <InsertItemTemplate>
                                <tr>
                                    <td>
                                        Gift description:
                                        &nbsp;<asp:TextBox ID="desc_txt" runat="server" Text='<%#Bind("Description") %>' Width="150px" CssClass="InputStyle" />
                                        
                                        <asp:RequiredFieldValidator ID="desc_rVal" runat="server" ValidationGroup="gift_vGroup" ControlToValidate="desc_txt" ErrorMessage="Description is required." Display="None" />
                                        <asp:ValidatorCalloutExtender ID="desc_vcExt" runat="server" TargetControlID="desc_rVal" WarningIconImageUrl="warningIcon.png"
                                              CloseImageUrl="closeIcon.png" CssClass="ValidatorCalloutStyle" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="complete_cbx" runat="server" Checked='<%#Bind("Complete") %>' Text="Complete" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        Notes:
                                        <br />
                                        <asp:TextBox ID="notes_txt" runat="server" Text='<%#Bind("Notes") %>' TextMode="MultiLine" Wrap="true" Width="360px" Height="80px" CssClass="InputStyle" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <span class="ModalDivider">&nbsp;</span>
                                        <br />
                                        
                                        <asp:Button ID="insert_btn" runat="server" CausesValidation="true" ValidationGroup="gift_vGroup" CommandName="Insert" Text="Add" CssClass="ButtonStyle" />
                                        &nbsp;&nbsp;
                                        <asp:Button ID="cancel_btn" runat="server" CausesValidation="false" CommandName="Cancel" Text="Cancel" CssClass="ButtonStyle" />
                                    </td>
                                </tr>
                        </InsertItemTemplate>
                        
                        <ItemTemplate>
                                <tr>
                                    <td>
                                        Gift description:
                                        &nbsp;<asp:Label ID="desc_lbl" runat="server" Text='<%#Bind("Description") %>' CssClass="FormViewLbl" />
                                    </td>
                                    <td>
                                        Complete: 
                                        &nbsp;<asp:Label ID="complete_lbl" runat="server" Text='<%#formatCompleteText(eval("Complete")) %>' CssClass="FormViewLbl" />
         
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        Notes:
                                        &nbsp;<asp:Label id="notes_lbl" runat="server" Text='<%#formatNullField(eval("Notes")) %>' CssClass="FormViewLbl" Font-Italic="true" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <span class="ModalDivider">&nbsp;</span>
                                        <br />
                                        
                                        <asp:Button ID="edit_btn" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" CssClass="ButtonStyle" />
                                        &nbsp;&nbsp;
                                        <asp:Button ID="delete_btn" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"
                                            OnClientClick="return confirm('Are you sure you want to delete this gift?');" CssClass="ButtonStyle" />
                                        &nbsp;&nbsp;
                                        <asp:Button ID="cancel_btn" runat="server" CausesValidation="false" CommandName="Cancel" Text="Cancel" CssClass="ButtonStyle" />
                                    </td>
                                </tr>
                        </ItemTemplate>
                        
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                        
                    </asp:FormView>
                    <%--/Gift FormView--%>
                    
                    <%--LEFT OFF HERE, 01/31/2012--%>
                    <%--Images Management and FormView--%>
                    <asp:Panel ID="images_pnl" runat="server">
                    
                        <asp:Label ID="gift_lbl" runat="server" Text='<%#formatGiftHeader() %>' />
                        <span class="ModalDivider">&nbsp;</span>
                        <br />
                        
                        <table class="modal_options_area">
                            <tr>
                                <td>
                                    <asp:DropDownList ID="images_ddl" runat="server" DataTextField="ImageTitle" DataValueField="ID" AutoPostBack="true" CssClass="InputStyle"
                                        OnSelectedIndexChanged="imagesSelectedIndexChanged" />
                                </td>
                                <td>
                                    <asp:Literal ID="prompt_lit" runat="server" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <span style="text-align:right;display:block;">
                            <asp:Button ID="outerCancel_btn" runat="server" CssClass="ButtonStyle" Text="Cancel" OnClick="outerCancelClick" />
                        </span>
                        
                        <%-- Ref BLOBSInDB (and 4Guys article http://www.4guysfromrolla.com/articles/120606-1.aspx)--%>
                        <asp:FormView ID="images_fv" runat="server" DataKeyNames="ID" DataSourceID="image_sds" Width="100%">
                            
                            <HeaderTemplate>
                                <table class="FormViewTbl">
                            </HeaderTemplate>
                            
                            <EditItemTemplate>
                                    <tr>
                                        <td>
                                            Image Title:
                                            &nbsp;<asp:TextBox ID="imgTitle_txt" runat="server" Text='<%#Bind("ImageTitle") %>' Width="150px" CssClass="InputStyle" />
                                            
                                            <asp:RequiredFieldValidator ID="imgTitle_rVal" runat="server" ValidationGroup="images_vGroup" ControlToValidate="imgTitle_txt" ErrorMessage="Image title is required." Display="None" />
                                            <asp:ValidatorCalloutExtender ID="imgTitle_vcExt" runat="server" TargetControlID="imgTitle_rVal" WarningIconImageUrl="warningIcon.png"
                                                  CloseImageUrl="closeIcon.png" CssClass="ValidatorCalloutStyle" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Image: 
                                            <asp:HiddenField ID="image_hField" runat="server" Value='<%#Bind("ImageData") %>' />
                                            &nbsp;<asp:Label ID="imageData_lbl" runat="server" Text='<%#eval("ImageTitle") %>' CssClass="FormViewLbl" />
                                            
                                            <br /><%--<asp:FileUpload ID="image_uploader" runat="server" CssClass="InputStyle" Width="150px" />--%>
                                            <asp:AsyncFileUpload ID="image_uploader" runat="server" CssClass="InputStyle" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span class="ModalDivider">&nbsp;</span>
                                            <br />
                                            
                                            <asp:Button ID="update_btn" runat="server" CausesValidation="true" ValidationGroup="gift_vGroup" CommandName="Update" Text="Update" CssClass="ButtonStyle" />
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
                                            
                                            <asp:RequiredFieldValidator ID="imgTitle_rVal" runat="server" ValidationGroup="images_vGroup" ControlToValidate="imgTitle_txt" ErrorMessage="Image title is required." Display="None" />
                                            <asp:ValidatorCalloutExtender ID="imgTitle_vcExt" runat="server" TargetControlID="imgTitle_rVal" WarningIconImageUrl="warningIcon.png"
                                                  CloseImageUrl="closeIcon.png" CssClass="ValidatorCalloutStyle" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Image: 
                                            <asp:HiddenField ID="image_hField" runat="server" Value='<%#Bind("ImageData") %>' />
                                            &nbsp;<asp:Label ID="imageData_lbl" runat="server" CssClass="FormViewLbl" />
                                            
                                            <br /><%--<asp:FileUpload ID="image_uploader" runat="server" CssClass="InputStyle" Width="150px" />--%>
                                            <asp:AsyncFileUpload ID="image_uploader" runat="server" CssClass="InputStyle" />
                                        
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span class="ModalDivider">&nbsp;</span>
                                            <br />
                                            
                                            <asp:Button ID="insert_btn" runat="server" CausesValidation="true" ValidationGroup="images_vGroup" CommandName="Insert" Text="Add" CssClass="ButtonStyle" />
                                            &nbsp;&nbsp;
                                            <asp:Button ID="cancel_btn" runat="server" CausesValidation="false" CommandName="Cancel" Text="Cancel" CssClass="ButtonStyle" />
                                        </td>
                                    </tr>
                            </InsertItemTemplate>
                            
                            <ItemTemplate>
                                    <tr>
                                        <td>
                                            Image Title:
                                            &nbsp;<asp:Label ID="imageData_lbl" runat="server" Text='<%#Bind("ImageTitle") %>' CssClass="FormViewLbl" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            MIME Type: 
                                            &nbsp;<asp:Label ID="mimeType_lbl" runat="server" Text='<%#Bind("MIMEType") %>' CssClass="FormViewLbl" />
                                        
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span class="ModalDivider">&nbsp;</span>
                                            <br />
                                            
                                            <asp:Button ID="edit_btn" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" CssClass="ButtonStyle" />
                                            &nbsp;&nbsp;
                                            <asp:Button ID="delete_btn" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"
                                                OnClientClick="return confirm('Are you sure you want to delete this image?');" CssClass="ButtonStyle" />
                                            &nbsp;&nbsp;
                                            <asp:Button ID="cancel_btn" runat="server" CausesValidation="false" CommandName="Cancel" Text="Cancel" CssClass="ButtonStyle" />
                                        </td>
                                    </tr>
                            </ItemTemplate>
                            
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                            
                        </asp:FormView>
                    
                    </asp:Panel>
                    <%--/Images Management and FormView--%>
                    
                </ContentTemplate>
                
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="gift_fv" EventName="ModeChanged" />
                    <asp:AsyncPostBackTrigger ControlID="gift_fv" EventName="ItemInserted" />
                    <asp:AsyncPostBackTrigger ControlID="gift_fv" EventName="ItemUpdated" />
                    <asp:AsyncPostBackTrigger ControlID="gift_fv" EventName="ItemCommand" />
                    
                    <asp:AsyncPostBackTrigger ControlID="recipient_fv" EventName="ItemInserted" />
                    <asp:AsyncPostBackTrigger ControlID="recipient_fv" EventName="ItemUpdated" />
                    <asp:AsyncPostBackTrigger ControlID="recipient_fv" EventName="ItemCommand" />
                    <asp:AsyncPostBackTrigger ControlID="recipient_fv" EventName="ModeChanged" />
                    
                    <asp:AsyncPostBackTrigger ControlID="category_fv" EventName="ModeChanged" />
                    <asp:AsyncPostBackTrigger ControlID="category_fv" EventName="ItemCommand" />
                    <asp:AsyncPostBackTrigger ControlID="category_fv" EventName="ItemInserted" />
                    <asp:AsyncPostBackTrigger ControlID="category_fv" EventName="ItemUpdated" />
                    <asp:AsyncPostBackTrigger ControlID="addCategory_btn" EventName="Click" />
                    
                    <asp:AsyncPostBackTrigger ControlID="images_fv" EventName="ModeChanged" />
                    <asp:AsyncPostBackTrigger ControlID="images_fv" EventName="ItemInserted" />
                    <asp:AsyncPostBackTrigger ControlID="images_fv" EventName="ItemUpdated" />
                    <asp:AsyncPostBackTrigger ControlID="images_fv" EventName="ItemDeleted" />
                    <asp:AsyncPostBackTrigger ControlID="images_fv" EventName="ItemCommand" />
                    <asp:AsyncPostBackTrigger ControlID="images_ddl" EventName="SelectedIndexChanged" />
                    
                </Triggers>
            
            </asp:UpdatePanel>

        </asp:Panel>
    
        <%-- ************************************************************************--%>
                        
        <div id="outer_div">
            
            <table id="outer_table">
            
                <tr>
                    <td>
                    
                        <table class="options_area">
                            <tr>
                                <td>
                                    <h3>Welcome to Gift Lister!</h3>
                                    <p>Start by adding categories to which to add recipients. You can then add gifts you intend to give to each recipient, 
                                    and check them off as you get them, or check off the entire recipient.</p>
                                    <p>Once you create your list, you can generate a printable version of it. You can select to print it reflecting what you've 
                                    checked off in this application, or as a checklist which you can check off by hand.</p>
                                    
                                </td>
                                <td align="right" style="border-left: solid 2px #9a9566; padding-left: 10px;">
                                    <asp:Button ID="addCategory_btn" runat="server" Text="+ Category" CssClass="ButtonStyle" OnClick="addCategoryClick" />
                                    <br />
                                    
                                    <%--Print options (only visible when the gift list is not completely empty)--%>
                                    <asp:Panel ID="print_pnl" runat="server" Visible="false">
                                        <span class="Divider">&nbsp;</span>
                                        <br />

                                        <table class="PrintPnlStyle">
                                            <tr>
                                                <td>
                                                    <b>List Title:</b>
                                                    <asp:TextBox ID="listTitle_txt" runat="server" CssClass="InputStyle" />
                                                    <br />
                                                    <asp:RadioButtonList ID="print_rbList" runat="server" RepeatDirection="Horizontal">
                                                        <asp:ListItem Text="Checklist" Value="Checklist" Selected="True" />
                                                        <asp:ListItem Text="Reflect Completion" Value="Completion" />
                                                    </asp:RadioButtonList>

                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <asp:Button ID="printPage_btn" runat="server" Text="Printable List &raquo;" CssClass="ButtonStyle" OnClick="printClick" />
                                                </td>
                                            </tr>
                                        </table>
                                        
                                        <br />
                                        <asp:UpdatePanel ID="retain_updatePnl" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:Panel ID="retainCbx_pnl" runat="server">
                                                    <asp:CheckBox ID="retainGiftStates_cbx" runat="server" Text="Retain Child States" AutoPostBack="true" />
                                                </asp:Panel>
                                                
                                                <asp:HoverMenuExtender ID="retain_hmExt" runat="server" TargetControlID="retainCbx_pnl" PopupControlID="retain_pnl"
                                                    Popupposition="Right" HoverDelay="0" />
                                                <asp:Panel ID="retain_pnl" runat="server" CssClass="ToolTipStyle">
                                                    Check this to retain the status of individually checked gifts after unchecking an entire recipient.
                                                </asp:Panel>
                                                
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <%--Christmas countdown--%>
                                <td colspan="2">
                                    <span class="Divider">&nbsp;</span>
                                    <br />
                                    
                                    <asp:UpdatePanel ID="countdown_updatePnl" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:Literal ID="countdown_lit" runat="server" />                                      
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="countdown_timer" EventName="Tick" />
                                            <asp:AsyncPostBackTrigger ControlID="xmas_timer" EventName="Tick" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                                                        
                                    <asp:Timer ID="countdown_timer" runat="server" Interval="1000" />
                                    <asp:Timer ID="xmas_timer" runat="server" Interval="1000" enabled="false" />
                                </td>
                                
                            </tr>
                        </table>
                        
                    </td>
                </tr>
                
                <tr>
                    <td>
                        <span class="Divider">&nbsp;</span>
                    </td>
                </tr>
                
                <tr>
                    <td>
                        
                        <%--THE LIST--%>

                        <asp:UpdatePanel ID="cat_updatePnl" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                            
                            <%--Click event is generated in JavaScript, updating the UpdatePanel and effectively loading all dynamically generated controls.--%>
                            <asp:Button ID="postback_btn" runat="server" Height="1" Width="1" style="visibility:hidden;"/>

                                <asp:Repeater ID="cat_repeater" runat="server" >
                                    <ItemTemplate>
                                        
                                        <asp:HiddenField ID="id_hField" runat="server" Value='<%#eval("ID") %>' />
                                        <table width="100%" class="CPanelStyle">
                                            <tr>
                                                <td>
                                                    <asp:CollapsiblePanelExtender id="cat_cpExt" runat="server" TargetControlID="cat_pnl" CollapsedSize="0" 
                                                        Collapsed="false" CollapsedImage="collapsed.png" ExpandedImage="expanded.png" ExpandControlID="cpControl_pnl" 
                                                        CollapseControlID="cpControl_pnl" TextLabelID="cpControl_lbl" ImageControlID="cpControl_img" CollapsedText='<%#configCPText(eval("Title"), eval("ID"), False)%>' 
                                                        ExpandedText='<%#configCPText(eval("Title"), eval("ID"), True)%>' />
                                                    
                                                    <asp:Panel ID="cpControl_pnl" runat="server" Visible='<%#itemVisible(eval("ID"), "Recipients", "CategoryID", False) %>'>
                                                        <asp:Image ID="cpControl_img" runat="server" ImageUrl="expanded.png" />
                                                        <asp:Label ID="cpControl_lbl" runat="server" Text='<%#configCPText(eval("Title"), eval("ID"), True) %>' CssClass="CPanelText" />
                                                        &nbsp;<asp:Image ID="checkmark_img" runat="server" ImageUrl="checkmarkCategory.png" Visible="false" />
                                                    </asp:Panel>
                                                    
                                                    <asp:Label ID="emptyCat_lbl" runat="server" Text='<%#configEmptyCatText(eval("Title"))%>' visible='<%#itemVisible(eval("ID"), "Recipients", "CategoryID", True) %>' Font-Italic="true" CssClass="CPanelText"/>
                                                </td>
                                                <td align="right">
                                                    <asp:Button ID="editCategory_btn" runat="server" Text="Edit Category" CssClass="ButtonStyle" OnClick="editCategoryClick" />
                                                    &nbsp;&nbsp;<asp:Button ID="addRecipient_btn" runat="server" Text="+ Recipient" CssClass="ButtonStyle" OnClick="addRecipientClick" />
                                                </td>
                                            </tr>
                                        </table>
                                        
                                        
                                        
                                        <asp:Panel ID="cat_pnl" runat="server">
                                            
                                            <asp:GridView ID="recipients_gv" runat="server" CssClass="GVStyle" DataKeyNames="ID, CategoryID" HeaderStyle-CssClass="GVHeaderStyle" RowStyle-CssClass="GVItemStyle"
                                                AutoGenerateColumns="false" GridLines="none" AllowPaging="false" OnRowDataBound="recipientsRowDataBound" OnSelectedIndexChanged="recipientSelectedIndexChanged">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Name" SortExpression="Last Name" ItemStyle-CssClass="GVNameStyle">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="name_lBtn" runat="server" Text='<%#formatNameText(eval("FirstName"), eval("LastName")) %>' CommandName="Select" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    
                                                    <asp:TemplateField HeaderText="Gifts" ItemStyle-Width="450px">
                                                        <ItemTemplate>
                                                        
                                                            <table width="100%" class="InnerCPanelStyle">
                                                                <tr>
                                                                    <td>
                                                                        <asp:CollapsiblePanelExtender id="gifts_cpExt" runat="server" TargetControlID="gifts_pnl" CollapsedSize="0" 
                                                                            Collapsed="true" CollapsedImage="collapsed.png" ExpandedImage="expanded.png" ExpandControlID="cpControl_pnl2" 
                                                                            CollapseControlID="cpControl_pnl2" TextLabelID="cpControl_lbl2" ImageControlID="cpControl_img2" CollapsedText='<%#configGiftsCPText(eval("ID"), False)%>'
                                                                            ExpandedText='<%#configGiftsCPText(eval("ID"), True) %>' />
                                                                            
                                                                        <asp:Panel ID="cpControl_pnl2" runat="server" Visible='<%#itemVisible(eval("ID"), "Gifts", "RecipientID", False) %>'>
                                                                            <asp:Image ID="cpControl_img2" runat="server" ImageUrl="collapsed.png" />
                                                                            <asp:Label ID="cpControl_lbl2" runat="server" Text='<%#configGiftsCPText(eval("ID"), True) %>' CssClass="InnerCPanelText" />
                                                                        </asp:Panel>
                                                                        
                                                                        <asp:Label ID="emptyRecipient_lbl" runat="server" Text="No gifts yet" CssClass="InnerEmptyText" visible='<%#itemVisible(eval("ID"), "Gifts", "RecipientID", True) %>' />
                                                                    </td>
                                                                    <td align="right">
                                                                        <asp:Button ID="addGift_btn" runat="server" Text="+ Gift" CssClass="ButtonStyle" OnClick="addGiftClick" />
                                                                    </td>
                                                                </tr>
                                                            
                                                            </table>
  
                                                            <asp:Panel ID="gifts_pnl" runat="server">
                                                            
                                                                <asp:GridView ID="gifts_gv" runat="server" DataKeyNames="ID, RecipientID" HeaderStyle-CssClass="GVInnerHeaderStyle" RowStyle-CssClass="GVInnerItemStyle"
                                                                    Gridlines="None" AutoGenerateColumns="false" AllowPaging="false" Width="450px" OnRowDataBound="giftsRowDataBound" OnSelectedIndexChanged="giftSelectedIndexChanged">
                                                                    <Columns>
                                                                        
                                                                        <asp:TemplateField ItemStyle-CssClass="GVInnerItemButtonStyle" HeaderText="Description">
                                                                            <ItemTemplate>
                                                                                <asp:LinkButton ID="gift_lBtn" runat="server" Text='<%#eval("Description") %>' CommandName="Select" />
                                                                                
                                                                                <asp:HoverMenuExtender ID="content_hmExt" runat="server" TargetControlID="gift_lBtn" PopupControlID="tooltip_pnl" 
                                                                                    PopupPosition="Right" OffsetY="1" HoverDelay="0" />
                                                                                    
                                                                                <asp:Panel ID="tooltip_pnl" runat="server" CssClass='<%#getToolTipClass(eval("Complete")) %>' Visible='<%#isTooltipVisible(eval("Notes")) %>'>
                                                                                    <asp:Literal ID="tooltip_lit" runat="server" Text='<%#formatToolTipText(eval("Notes"))%>' />
                                                                                </asp:Panel>
                                                                                
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        
                                                                        <asp:TemplateField ItemStyle-CssClass="GVInnerItemButtonStyle" HeaderText="Images">
                                                                            <ItemTemplate>
                                                                                <asp:LinkButton ID="images_lBtn" runat="server" Text='<%#formatImagesText(eval("ID")) %>' OnClick="manageImagesClick" />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        
                                                                        <asp:TemplateField HeaderText="Complete" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Right">
                                                                            <ItemTemplate>
                                                                                <asp:CheckBox ID="giftComplete_cbx" runat="server" Checked='<%#eval("Complete") %>' AutoPostBack="true" OnCheckedChanged="giftCompleteCheckedChanged" />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        
                                                                    </Columns>    
                                                                </asp:GridView>
                                                            
                                                            </asp:Panel>
                                                        
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    
                                                    <asp:TemplateField HeaderText="Complete" ItemStyle-Width="50px" ItemStyle-CssClass="GVNameStyle" ItemStyle-HorizontalAlign="Right">
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="recipientComplete_cbx" runat="server" AutoPostBack="true" OnCheckedChanged="recipientCompleteCheckedChanged" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    
                                                </Columns>
                                            </asp:GridView>
                                            
                                        </asp:Panel>
                                        <br />
                                        
                                    </ItemTemplate>
                                </asp:Repeater>
                                
                            </ContentTemplate>
                            
                            <triggers>
                                <asp:AsyncPostBackTrigger ControlID="postback_btn" EventName="Click" />
                            </triggers>
                            
                        </asp:UpdatePanel>
                    </td>
                
                </tr>
            </table>
            
            <%-- ************************************************************************--%>
            <asp:SqlDataSource ID="category_sds" runat="server" ConnectionString="<%$ ConnectionStrings:GiftListConnectionString %>" 
                SelectCommand="SELECT ID, Title FROM Categories WHERE ID = @ID" 
                InsertCommand="INSERT INTO Categories (Title) VALUES (@Title);SET @NewID = SCOPE_IDENTITY()"
                UpdateCommand="UPDATE Categories SET Title = @Title WHERE ID = @ID"
                DeleteCommand="DELETE FROM Categories WHERE ID = @ID"
                >
            
                <SelectParameters>
                    <asp:SessionParameter Name="ID" SessionField="SelectedCategory" Type="Int32" />
                </SelectParameters>
                
                <DeleteParameters>
                    <asp:Parameter Name="ID" Type="Int32" />
                </DeleteParameters>
                
                <InsertParameters>
                    <asp:Parameter Name="Title" Type="String" />
                    <asp:Parameter Name="NewID" Direction="Output" Type="Int32" Size="4" />
                </InsertParameters>
                
                <UpdateParameters>
                    <asp:Parameter Name="ID" Type="Int32" />
                    <asp:Parameter Name="Title" Type="String" />
                </UpdateParameters>
                
            </asp:SqlDataSource>
            
            
            <asp:SqlDataSource id="recipient_sds" runat="server" ConnectionString="<%$ ConnectionStrings:GiftListConnectionString %>"
                SelectCommand="SELECT Recipients.ID, FirstName, LastName, CategoryID, Title FROM Recipients INNER JOIN Categories on Recipients.CategoryID = Categories.ID WHERE Recipients.ID = @ID"
                InsertCommand="INSERT INTO Recipients (FirstName, LastName, CategoryID) VALUES (@FirstName, @LastName, @CategoryID);SET @NewID = SCOPE_IDENTITY()"
                UpdateCommand="UPDATE Recipients SET FirstName = @FirstName, LastName = @LastName, CategoryID = @CategoryID WHERE ID = @ID"
                DeleteCommand="DELETE FROM Recipients WHERE ID = @ID"
                >
                
                <SelectParameters>
                    <asp:SessionParameter Name="ID" SessionField="SelectedRecipient" Type="Int32" />
                </SelectParameters>
                
                <DeleteParameters>
                    <asp:Parameter Name="ID" Type="Int32" />
                </DeleteParameters>
                
                <UpdateParameters>
                    <asp:Parameter Name="ID" Type="Int32" />
                    <asp:Parameter Name="FirstName" Type="String" />
                    <asp:Parameter Name="LastName" Type="String" />
                    <asp:Parameter Name="CategoryID" Type="Int32" />
                </UpdateParameters>
                
                <InsertParameters>
                    <asp:Parameter Name="FirstName" Type="String" />
                    <asp:Parameter Name="LastName" Type="String" />
                    <asp:SessionParameter Name="CategoryID" SessionField="SelectedRecipientCategory" Type="Int32" />
                    <asp:Parameter Name="NewID" Type="Int32" Size="4" Direction="Output" />
                </InsertParameters>
                
            </asp:SqlDataSource>
                        
            <asp:SqlDataSource ID="gift_sds" runat="server" ConnectionString="<%$ ConnectionStrings:GiftListConnectionString %>"
                SelectCommand="SELECT Gifts.ID, Description, Notes, Complete, FirstName, LastName FROM Gifts INNER JOIN Recipients ON Gifts.RecipientID = Recipients.ID WHERE Gifts.ID = @ID"
                InsertCommand="INSERT INTO Gifts (Description, Notes, Complete, RecipientID) VALUES (@Description, @Notes, @Complete, @RecipientID);SET @NewID = SCOPE_IDENTITY()"
                UpdateCommand="UPDATE Gifts SET Description = @Description, Notes = @Notes, Complete = @Complete WHERE ID = @ID"
                DeleteCommand="DELETE FROM Gifts WHERE ID = @ID"
                >
                
                <SelectParameters>
                    <asp:SessionParameter Name="ID" SessionField="SelectedGift" Type="Int32" />
                </SelectParameters>
                
                <DeleteParameters>
                    <asp:Parameter Name="ID" Type="Int32" />
                </DeleteParameters>
                
                <UpdateParameters>
                    <asp:Parameter Name="ID" type="Int32" />
                    <asp:Parameter Name="Description" Type="String" />
                    <asp:Parameter Name="Notes" Type="String" />
                    <asp:Parameter Name="Complete" Type="Boolean" />
                </UpdateParameters>
                
                <InsertParameters>
                    <asp:Parameter Name="Description" Type="String" />
                    <asp:Parameter Name="Notes" Type="String" />
                    <asp:Parameter Name="Complete" Type="Boolean" />
                    <asp:SessionParameter Name="RecipientID" SessionField="SelectedGiftRecipient" Type="Int32" />
                    <asp:Parameter Name="NewID" Type="Int32" Size="4" Direction="Output" />
                </InsertParameters>
                
            </asp:SqlDataSource>
            
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

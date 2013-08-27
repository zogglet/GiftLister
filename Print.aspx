<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Print.aspx.vb" Inherits="Print" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Gift Lister: A Christmas Gift Recipient Manager</title>
    
    <link href="favicon.ico" rel="icon" type="image/x-icon" />
    <link href="style.css" rel="stylesheet" type="text/css" />
    
</head>

<body>
    <form id="form1" runat="server">
        
        <div id="outer_div">
            
            <table id="outer_table">
 
                <tr>
                    <td>
                        <table class="options_area">
                            <tr>
                                <td>
                                    <asp:Literal ID="title_lit" runat="server" />
                                    <%--This is in a separate literal in order to toggle it if nothing exists--%>
                                    <asp:Literal ID="summary_lit" runat="server" />
                                    <br />
                                    
                                    <asp:Panel ID="printItems_pnl" runat="server" HorizontalAlign="Center">
                                        <asp:Button ID="back_btn" runat="server" Text="&laquo; Back" CssClass="ButtonStyle" OnClick="backClick" />
                                        &nbsp;&nbsp;<asp:Button ID="print_btn" runat="server" Text="Print" CssClass="ButtonStyle" OnClientClick="window.print();return false" />
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                        <br />
                    </td>
                </tr>
                
                <tr>
                    <td>
                        <asp:Repeater ID="cat_repeater" runat="server" >
                            <ItemTemplate>
                                
                                <asp:HiddenField ID="id_hField" runat="server" Value='<%#eval("ID") %>' />
                                
                                <asp:Panel ID="catTitle_pnl" runat="server" CssClass="CPanelStyle">
                                    <asp:Literal ID="cat_lit" runat="server" Text='<%#configCatText(eval("Title"), eval("ID"))%>' />
                                    &nbsp;<asp:Image ID="catComplete_img" runat="server" Visible='<%#itemVisible(eval("ID"), "Recipients", "CategoryID") %>' />
                                </asp:Panel>
                                
                                    
                                    <asp:GridView ID="recipients_gv" runat="server" CssClass="GVStyle" DataKeyNames="ID, CategoryID" HeaderStyle-CssClass="GVHeaderStyle" RowStyle-CssClass="GVItemStyle"
                                        AutoGenerateColumns="false" GridLines="none" AllowPaging="false" OnRowDataBound="recipientsRowDataBound">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Name" SortExpression="Last Name" ItemStyle-CssClass="GVNameStyle">
                                                <ItemTemplate>
                                                    <asp:Literal ID="name_lit" runat="server" Text='<%#formatNameText(eval("FirstName"), eval("LastName")) %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            
                                            <asp:TemplateField HeaderText="Gifts" ItemStyle-Width="450px">
                                                <ItemTemplate>
               
                                                    <asp:Literal ID="recipient_lit" runat="server" Text='<%#configRecipText(eval("ID")) %>' />

                                                    <asp:GridView ID="gifts_gv" runat="server" DataKeyNames="ID, RecipientID" HeaderStyle-CssClass="GVInnerHeaderStyle" RowStyle-CssClass="GVInnerItemStyle"
                                                        Gridlines="None" AutoGenerateColumns="false" AllowPaging="false" Width="450px" OnRowDataBound="giftsRowDataBound">
                                                        <Columns>
                                                            
                                                            <asp:TemplateField ItemStyle-CssClass="GVInnerItemButtonStyle" HeaderText="Description">
                                                                <ItemTemplate>
                                                                    <asp:Literal ID="gift_lit" runat="server" Text='<%#eval("Description") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            
                                                            <asp:TemplateField ItemStyle-CssClass="GVInnerItemStyle" HeaderText="Notes" ItemStyle-Width="225px">
                                                                <ItemTemplate>
                                                                    <asp:Literal ID="notes_lit" runat="server" Text='<%#formatNullField(eval("Notes"))%>'  />
                                                                    
                                                                    <asp:DataList ID="images_dList" runat="server" RepeatDirection="Vertical" RepeatColumns="1" Visible="false" 
                                                                        OnItemDataBound="imagesItemDataBound">
                                                                        <HeaderTemplate>
                                                                            <br />
                                                                            <span class="GVInnerImagesHeaderStyle">Images:</span>
                                                                        </HeaderTemplate>
                                                                        <ItemTemplate>
                                                                            <%--Images are rendered here--%>
                                                                            <asp:Panel ID="image_pnl" runat="server" CssClass="ListImagePnlStyle"></asp:Panel>
                                                                        </ItemTemplate>
                                                                    </asp:DataList>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            
                                                            <asp:TemplateField HeaderText="Complete" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Right">
                                                                <ItemTemplate>
                                                                    <asp:Image ID="giftComplete_img" runat="server" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            
                                                        </Columns>    
                                                    </asp:GridView>
                                                    <br />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            
                                            <asp:TemplateField HeaderText="Complete" ItemStyle-Width="50px" ItemStyle-CssClass="GVNameStyle" ItemStyle-HorizontalAlign="Right">
                                                <ItemTemplate>
                                                    <asp:Image ID="recipientComplete_img" runat="server" Visible='<%#itemVisible(eval("ID"), "Gifts", "RecipientID")%>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            
                                        </Columns>
                                    </asp:GridView>
                                    
                                <br />
                                
                            </ItemTemplate>
                        </asp:Repeater>

                    </td>
                
                </tr>
            </table>
            
            
        </div>
        
        <div class="PrintFooter">
            Copyright &copy; 2011, <a href="mailto:maggy@zogglet.com?subject=About your awesome Christmas Gift Lister">Maggy Maffia</a>
        </div>
        
        
    </form>
</body>
</html>

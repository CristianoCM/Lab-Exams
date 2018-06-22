<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListagemExames.aspx.cs" Inherits="LaboratorioExames.ListagemExames" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="refresh" content="300" />
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Exames Laboratório</title>

    <style type="text/css">
      .hiddencol
      {
        display: none;
      }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="Logo">
            <div style="float:right;"><img src="Imagens/logomarca_small_transp.png" style="width: 73%; height: 73%;" /></div>
        </div>
        <div id="Title" style="background-color:black; height:61px; color: whitesmoke; font-family:'Trebuchet MS', 'Lucida Sans Unicode', 'Lucida Grande', 'Lucida Sans', Arial, sans-serif">
            <div style="float:left; padding-right: 15px; left: 20px; top: 15px; position:absolute;"><img src="Imagens/icon-exames-lab2.png" /></div>
            <div style="float:left; position: absolute; left: 80px;"><h2>Exames Laboratório</h2></div>
            <div class="row" style="position: absolute; left: 350px;">
                <table>
                    <tr>
                        <td>
                            <img src="Imagens/calendar3.png" />
                            <img src="Imagens/De.png" />
                            <div class="form-group col-md-4">
                                <asp:TextBox ID="txtDe" runat="server" type="date"></asp:TextBox>
                            </div>
                        </td>
                        <td></td>
                        <td>
                            <img src="Imagens/calendar3.png" />
                            <img src="Imagens/Ate.png" />
                            <div class="form-group col-md-4">
                                <asp:TextBox ID="txtAte" runat="server" type="date"></asp:TextBox>
                            </div>
                        </td>
                        <td></td>
                        <td style="position:absolute; bottom: 3px;">
                            <asp:Button ID="btnPesquisa" runat="server" Text="Filtrar" Font-Names="Trebuchet MS" Width="90px" OnClick="btnPesquisa_Click" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <br />
        <div id="Table">
            <%-- GRID VIEW --%>
            <div class="row" style="display:table; margin: 0 auto;">
                <asp:GridView runat="server" ID="grdExames" ShowHeaderWhenEmpty="true" EmptyDataText="Sem exames" AutoGenerateColumns="false" CellPadding="5"
                    BackColor="White" HeaderStyle-BackColor="#333333" HeaderStyle-Font-Size="Large" HeaderStyle-ForeColor="White" OnRowDataBound="grdExames_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="PORC" HeaderText="Porcentagem" ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol" />
                        <asp:BoundField DataField="STAT" HeaderText="👐" />
                        <asp:BoundField DataField="DH" HeaderText="Data/Hora" />
                        <asp:BoundField DataField="MEDIA" HeaderText="Média" />
                        <asp:BoundField DataField="PREV" HeaderText="Previsão" />
                        <asp:BoundField DataField="TEMP" HeaderText="Tempo" />
                        <asp:BoundField DataField="PED" HeaderText="Pedido" />
                        <asp:BoundField DataField="ATEND" HeaderText="Atend." />
                        <asp:BoundField DataField="PAC" HeaderText="Paciente" />
                        <asp:BoundField DataField="SET" HeaderText="     Setor     " />
                        <asp:BoundField DataField="EXA" HeaderText="Exame" />
                        <asp:BoundField DataField="USU" HeaderText="Usuario" ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </form>
</body>
</html>

Event Start

    Form.Caption = Form.Caption + ' - (' + Trim(&Pgmname) + ')'


btnalt.TooltipText = 'Alterar Nota Fiscal'
BtnExc.TooltipText = 'Excluir Nota Fiscal'
BtnDsp.TooltipText = 'Visualizar Nota Fiscal'
BtnExit.TooltipText = 'Sair'
BtnImp.TooltipText = 'Imprimir Relação de Notas Fiscais'
btnexcel.TooltipText = 'Gerar Relação de Notas Fiscais .XLS'
btncanc.TooltipText = 'Cancelar Nota Fiscal'
BtnNfe.TooltipText = 'Gerar arquivo da NF-e'
BtnDanfe.TooltipText = 'Pré Visualizar Danfe'

SETINHA.TooltipText  = 'Configurações'

for each
   where UsrCod = &Logon.UsrCod
   where UsrPerSeq = 1

   &UsrPerVenNf = UsrPerVenNf
   &UsrPerVenNfUpd = UsrPerVenNfUpd
   &UsrPerVenNfDsp = UsrPerVenNfDsp
   &UsrPerVenNfDlt = UsrPerVenNfDlt
   &UsrPerVenNfTot = UsrPerVenNfTot

endfor

if &UsrPerVenNf = 'N'
   Msg('Usuário não possui permissão de acesso a esta operação')
   return
endif


if  &UsrPerVenNfUpd = 'N' and &UsrPerVenNfDlt = 'N' and &UsrPerVenNfDsp = 'S' 
   btnnfe.Enabled = 0
   btncanc.Enabled = 0
endif

if &UsrPerVenNfDsp = 'N'
   BtnDsp.Enabled = 0
endif

if &UsrPerVenNfDlt = 'N'
   BtnExc.Enabled = 0
endif

&NfsSts = ''
&NfsTpTrn = 0


 //atribui configurações iniciais
    Call(PConfigNfs, &logon,&CnfNfsBold,&CnfNfsCidNom ,&CnfNfsCli ,&CnfNfsCorFonte ,&CnfNfsCorLinha ,&CnfNfsDtaEms ,&CnfNfsFont, &CnfNfsForNom, &CnfNfsItalic ,&CnfNfsNum ,&CnfNfsSeq ,&CnfNfsSer ,&CnfNfsTotNf ,&CnfNfsTotPrd ,&CnfNfsTpTrn ,&CnfNfsUfCod ,&CnfNfsUnderline ,&CnfNfsUsr ,&CnfNfsVenNom,&CnfNfsNumPed,&CnfNfsSts,&CnfNfsBseIcms,&CnfNfsVlrIcms,&CnfNfsBseSt,&CnfNfsVlrSt,&CnfNfsFrete,&CnfNfsOutDsp,&CnfNfsDsc,&CnfNfsBseClcIpi,&CnfNfsVlrIpi)
    do'Config'

if &UsrPerVenNfTot = 'N'
   LBLTOT.Visible       = 0
   &TotaPed.Visible     = 0
   NfsVlrTotNf.Visible  = 0
   NfsVlrTotPrd.Visible = 0
endif

if &UsrPerVenNfTot = 'S'
   LBLTOT.Visible       = 1
   &TotaPed.Visible     = 1
   NfsVlrTotNf.Visible  = 1
   NfsVlrTotPrd.Visible = 1
endif

&UfCod.Additem('','')
&UfCod = ''
   

for each
   where EmpCod = &Logon.EmpCod

   &EmpLogoNome = EmpLogoNome
   &EmpNomFan = EmpNomFan

   &EmpCnpj = EmpCnpj
   &EmpNom = EmpNom
   &EmpFone = EmpFone

   if EmpIEise = 'S'

         &EmpIe = 'ISENTO'

   else

         &EmpIe = EmpIE

   endif

   &EmpTpEndSeq = EmpTpEndSeq
   &EmpCnpj = EmpCnpj
   &EmpVerLei = EmpVerLei
   &EmpProcEmi = EmpProcEmi
   &EmpDirTxtNfe = EmpDirTxtNfe
   do'tipo'

   &End = upper(&TipoEnd.Trim())+space(1)+EmpEnd.Trim()+' - '+EmpEndComp.Trim()+','+EmpEndNum.Trim()+' - '+EmpEndBai.Trim()
   &End2 = EmpCep.Trim()+' - '+EmpCidade.Trim()+' - '+EmpUf.Trim()

endfor

    &PedDta = &Today
    &PedDta2 = &Today
endevent


Event 'Alt'
    if not null(NfsNum)
        if NfsSts = 'N'
            call(WNotaFiscal2,&Logon,NfsNum,NfsSer)
            refresh
        else
            msg('Nota Fiscal não pode ser alterada pois sua situação não permite!')
        endif
    endif
EndEvent  // 'Alt'


Event 'Exc'

if not null(NfsNum)
    &Mensagem = 'Deseja realmente EXCLUIR a nota fiscal nº. ' + trim(str(NfsNum)) + ' ?'
   Confirm(&Mensagem, N)

   if confirmed()
     Call(PVerStsNF, &Logon, NfsNum, NfsSer, &NfsSts2)

     if &NfsSts2 = 'N'
        Call(PExcluirNF,&Logon,NfsNum,NfsSer,NfsNumPed)
        refresh
     else
        msg('Nota Fiscal não pode ser cancelada pois sua situação não permite')
     endif

   endif

endif

EndEvent  // 'Exc'



Event 'Imp'
    &SdtNota.Clear()
    for each line
      &SdtNotaItem = new SdtNota.SdtNotaItem()
      &SdtNotaItem.NfsNum = NfsNum
      &SdtNotaItem.NfsSer = NfsSer
      &SdtNota.Add(&SdtNotaItem)
    endfor
    
    if &SdtNota.Count > 0
       call(RRelNf,&Logon,&SdtNota)
    endif
EndEvent  // 'Imp'

event load

&TotaPed += NfsVlrTotNf

endevent 

event refresh
    &Esc = 'N'
    &TotaPed = 0
    Call(PCommit) 
    Call(PImpRetEmissor, &Logon) // Importa os retornos do emissor e atualiza o status das notas fiscais
endevent


event &sel.Click
    for each line
        &Esc = &sel
    endfor
endevent



Event 'Excel'
   call('gxSelDir',&Arquivo,'C:\ ','Selecione o diretório',0)

   if not null(&arquivo)
      &arquivo += '\NotasFiscais.xls'

      &ret = deletefile(&arquivo)

      &Excel.Open(&arquivo)

      &x = 0

      if &CnfNfsNum = 1
         &x += 1
         &Excel.Cells(1,&x).Text = 'Nota Fiscal'
      endif

      if &CnfNfsSer = 1
         &x += 1
         &Excel.Cells(1,&x).Text = 'Série'
      endif

      if &CnfNfsDtaEms = 1
         &x += 1
         &Excel.Cells(1,&x).Text = 'Emissão'
      endif

      if &CnfNfsCli = 1
         &x += 1
         &Excel.Cells(1,&x).Text = 'Cliente'
      endif

      if &CnfNfsTotNf = 1
         &x += 1
         &Excel.Cells(1,&x).Text = 'Total da Nf'
      endif

      if &CnfNfsTotPrd = 1
         &x += 1
         &Excel.Cells(1,&x).text = 'Total dos Produtos'
      endif

      if &CnfNfsTpTrn = 1
         &x += 1
         &Excel.Cells(1,&x).text = 'Tipo de Transação'
      endif

      if &CnfNfsVenNom = 1
         &x += 1
         &Excel.Cells(1,&x).Text = 'Vendedor'
      endif

      if &CnfNfsForNom = 1
         &x += 1
         &Excel.Cells(1,&x).Text = 'Transportadora'
      endif

      if &CnfNfsCidNom = 1
         &x += 1
         &Excel.Cells(1,&x).Text = 'Cidade'
      endif

      if &CnfNfsUfCod= 1
         &x += 1
         &Excel.Cells(1,&x).Text = 'Estado'
      endif

      if &CnfNfsBseIcms = 1
         &x += 1
         &Excel.Cells(1,&x).Text = 'Base de Calculo do ICMS'
      endif

      if &CnfNfsVlrIcms = 1
         &x += 1
         &Excel.Cells(1,&x).Text = 'Valor do ICMS'
      endif

      if &CnfNfsBseSt = 1
         &x += 1
         &Excel.Cells(1,&x).Text = 'Base de Calculo da ST'
      endif

      if &CnfNfsVlrSt = 1
         &x += 1
         &Excel.Cells(1,&x).Text = 'Valor da ST'
      endif

      if &CnfNfsBseClcIpi = 1
         &x += 1
         &Excel.Cells(1,&x).Text = 'Base de Calculo do IPI'
      endif

      if &CnfNfsVlrIpi = 1
         &x += 1
         &Excel.Cells(1,&x).Text = 'Valor do IPI'
      endif

      if &CnfNfsOutDsp = 1
         &x += 1
         &Excel.Cells(1,&x).Text = 'Outras Despesas'
      endif

      if &CnfNfsFrete = 1
         &x += 1
         &Excel.Cells(1,&x).Text = 'Frete'
      endif

      if &CnfNfsDsc = 1
         &x += 1
         &Excel.Cells(1,&x).Text = 'Desconto'
      endif

      &y = 0
      &x = 2
      
      for each line

         if &CnfNfsNum = 1
             &y += 1
             &Excel.Cells(&x,&y).Number = NfsNum
          endif
    
          if &CnfNfsSer = 1
             &y += 1
             &Excel.Cells(&x,&y).Text = NfsSer
          endif
    
          if &CnfNfsDtaEms = 1
             &y += 1
             &Excel.Cells(&x,&y).Date = NfsDtaEms
          endif
    
          if &CnfNfsCli = 1
             &y += 1
             &Excel.Cells(&x,&y).Text = NfsCliNom
          endif
    
          if &CnfNfsTotNf = 1
             &y += 1
             &Excel.Cells(&x,&y).Number = NfsVlrTotNf
          endif
    
          if &CnfNfsTotPrd = 1
             &y += 1
             &Excel.Cells(&x,&y).Number = NfsVlrTotPrd
          endif
    
          if &CnfNfsTpTrn = 1
             &y += 1
             &Excel.Cells(&x,&y).text = NfsTipTrnDsc
          endif
    
          if &CnfNfsVenNom = 1
             &y += 1
             &Excel.Cells(&x,&y).Text = NfsVenNom
          endif
    
          if &CnfNfsForNom = 1
             &y += 1
             &Excel.Cells(&x,&y).Text = NfsTrpNom
          endif
    
          if &CnfNfsCidNom = 1
             &y += 1
             &Excel.Cells(&x,&y).Text = NfsCliCidNom
          endif
    
          if &CnfNfsUfCod= 1
             &y += 1
             &Excel.Cells(&x,&y).Text = NfsCliUfCod
          endif

          if  &CnfNfsBseIcms = 1
             &y += 1
             &Excel.Cells(&x,&y).Number = NfsBseClcIcms
          endif

          if  &CnfNfsVlrIcms = 1
             &y += 1
             &Excel.Cells(&x,&y).Number = NfsVlrIcms
          endif

          if  &CnfNfsBseSt = 1
             &y += 1
             &Excel.Cells(&x,&y).Number = NfsBseClcSt
          endif

          if  &CnfNfsVlrSt = 1
             &y += 1
             &Excel.Cells(&x,&y).Number = NfsVlrSt
          endif

          if  &CnfNfsBseClcIpi = 1
             &y += 1
             &Excel.Cells(&x,&y).Number = NfsBseClcIpi
          endif

          if  &CnfNfsVlrIpi = 1
             &y += 1
             &Excel.Cells(&x,&y).Number = NfsVlrIpi
          endif

          if &CnfNfsOutDsp = 1
             &y += 1
             &Excel.Cells(&x,&y).Number = NfsOutDsp
          endif

          if &CnfNfsFrete = 1
             &y += 1
             &Excel.Cells(&x,&y).Number = NfsVlrFrt
          endif

          if &CnfNfsDsc = 1
             &y += 1
             &Excel.Cells(&x,&y).Number = NfsVlrDsc
          endif
          
    

           &y = 0
           &x += 1
      endfor      

      &Excel.Save()
//      &Excel.Show()
      &Excel.Close()

      MSG('Arquivo Gerado com Sucesso !!!')
   else
      msg('Selecione um diretório válido')
   endif
EndEvent  // 'Excel'

Event 'Opcoes'
WCnfNf.Call(&Logon)
do'Config'
refresh
EndEvent  // 'Opcoes'


sub'Config'

  for each
   where CnfNfsSeq = 1
   where CnfNfsUsr = &Logon.UsrCod

    &CnfNfsSeq = CnfNfsSeq
    &CnfNfsUsr = CnfNfsUsr
    &CnfNfsNum = CnfNfsNum
    &CnfNfsSer = CnfNfsSer
    &CnfNfsDtaEms = CnfNfsDtaEms
    &CnfNfsCli = CnfNfsCli
    &CnfNfsTotNf = CnfNfsTotNf
    &CnfNfsTotPrd = CnfNfsTotPrd
    &CnfNfsTpTrn = CnfNfsTpTrn
    &CnfNfsVenNom = CnfNfsVenNom
    &CnfNfsForNom = CnfNfsForNom
    &CnfNfsUfCod = CnfNfsUfCod
    &CnfNfsCidNom = CnfNfsCidNom
    &CnfNfsBold = CnfNfsBold
    &CnfNfsUnderline = CnfNfsUnderline
    &CnfNfsItalic = CnfNfsItalic
    &CnfNfsCorLinha = CnfNfsCorLinha
    &CnfNfsCorFonte = CnfNfsCorFonte
    &CnfNfsFont = CnfNfsFont
    &CnfNfsNumPed = CnfNfsNumPed
    &CnfNfsSts = CnfNfsSts
    &CnfNfsBseIcms = CnfNfsBseIcms
    &CnfNfsVlrIcms = CnfNfsVlrIcms
    &CnfNfsBseSt = CnfNfsBseSt
    &CnfNfsVlrSt = CnfNfsVlrSt
    &CnfNfsFrete = CnfNfsFrete
    &CnfNfsOutDsp = CnfNfsOutDsp
    &CnfNfsDsc = CnfNfsDsc
    &CnfNfsBseClcIpi = CnfNfsBseClcIpi
    &CnfNfsVlrIpi = CnfNfsVlrIpi

 endfor

  NfsNum.Visible       = &CnfNfsNum
  NfsDtaEms.Visible    = &CnfNfsDtaEms
  NfsSer.Visible       = &CnfNfsSer
  NfsCliNom.Visible    = &CnfNfsCli
  NfsVlrTotNf.Visible  = &CnfNfsTotNf
  NfsVlrTotPrd.Visible = &CnfNfsTotPrd
  NfsTipTrnDsc.Visible = &CnfNfsTpTrn
  NfsVenNom.Visible    = &CnfNfsVenNom
  NfsTrpNom.Visible    = &CnfNfsForNom
  NfsCliCidNom.Visible = &CnfNfsCidNom
  NfsCliUfCod.Visible  = &CnfNfsUfCod
  NfsNumPed.Visible    = &CnfNfsNumPed
  NfsSts.Visible       = &CnfNfsSts
  NfsBseClcIcms.Visible= &CnfNfsBseIcms
  NfsVlrIcms.Visible   = &CnfNfsVlrIcms
  NfsBseClcSt.Visible  = &CnfNfsBseSt
  NfsVlrSt.Visible     = &CnfNfsVlrSt
  NfsVlrFrt.Visible    = &CnfNfsFrete
  NfsOutDsp.Visible    = &CnfNfsOutDsp
  NfsVlrDsc.Visible    = &CnfNfsDsc
  NfsBseClcIpi.Visible = &CnfNfsBseClcIpi
  NfsVlrIpi.Visible    = &CnfNfsVlrIpi

  if &UsrPerVenNfTot = 'N'
     NfsVlrTotNf.Visible  = 0
     NfsVlrTotPrd.Visible = 0
  endif
    
  if &UsrPerVenNfTot = 'S'
     NfsVlrTotNf.Visible  = 1
     NfsVlrTotPrd.Visible = 1
  endif

  NfsNum.FontName       = &CnfNfsFont
  NfsDtaEms.FontName    = &CnfNfsFont
  NfsSer.FontName       = &CnfNfsFont
  NfsCliNom.FontName    = &CnfNfsFont
  NfsVlrTotNf.FontName  = &CnfNfsFont
  NfsVlrTotPrd.FontName = &CnfNfsFont
  NfsTpTrn.FontName     = &CnfNfsFont
  NfsVenNom.FontName    = &CnfNfsFont
  NfsTrpNom.FontName    = &CnfNfsFont
  NfsCliCidNom.FontName = &CnfNfsFont
  NfsCliUfCod.FontName  = &CnfNfsFont
  NfsNumPed.FontName    = &CnfNfsFont
  NfsSts.FontName    = &CnfNfsFont
  NfsBseClcIcms.FontName        = &CnfNfsFont
  NfsVlrIcms.FontName           = &CnfNfsFont
  NfsBseClcSt.FontName          = &CnfNfsFont
  NfsVlrSt.FontName             = &CnfNfsFont
  NfsVlrFrt.FontName            = &CnfNfsFont
  NfsOutDsp.FontName         = &CnfNfsFont
  NfsVlrDsc.FontName         = &CnfNfsFont
  NfsBseClcIpi.FontName = &CnfNfsFont
  NfsVlrIpi.FontName    = &CnfNfsFont

  NfsNum.FontBold       = &CnfNfsBold
  NfsDtaEms.FontBold    = &CnfNfsBold
  NfsSer.FontBold       = &CnfNfsBold
  NfsCliNom.FontBold    = &CnfNfsBold
  NfsVlrTotNf.FontBold  = &CnfNfsBold
  NfsVlrTotPrd.FontBold = &CnfNfsBold
  NfsTipTrnDsc.FontBold     = &CnfNfsBold
  NfsVenNom.FontBold    = &CnfNfsBold
  NfsTrpNom.FontBold    = &CnfNfsBold
  NfsCliCidNom.FontBold = &CnfNfsBold
  NfsCliUfCod.FontBold  = &CnfNfsBold
  NfsNumPed.FontBold    = &CnfNfsBold
  NfsSts.FontBold    = &CnfNfsBold
  NfsBseClcIpi.FontBold = &CnfNfsBold
  NfsVlrIpi.FontBold    = &CnfNfsBold
  NfsBseClcIcms.FontBold       = &CnfNfsBold
  NfsVlrIcms.FontBold          = &CnfNfsBold
  NfsBseClcSt.FontBold         = &CnfNfsBold
  NfsVlrSt.FontBold            = &CnfNfsBold
  NfsVlrFrt.FontBold            = &CnfNfsBold
  NfsOutDsp.FontBold        = &CnfNfsBold
  NfsVlrDsc.FontBold        = &CnfNfsBold

  NfsNum.FontItalic       = &CnfNfsItalic
  NfsDtaEms.FontItalic    = &CnfNfsItalic
  NfsSer.FontItalic       = &CnfNfsItalic
  NfsCliNom.FontItalic    = &CnfNfsItalic
  NfsVlrTotNf.FontItalic  = &CnfNfsItalic
  NfsVlrTotPrd.FontItalic = &CnfNfsItalic
  NfsTipTrnDsc.FontItalic     = &CnfNfsItalic
  NfsVenNom.FontItalic    = &CnfNfsItalic
  NfsTrpNom.FontItalic    = &CnfNfsItalic
  NfsCliCidNom.FontItalic = &CnfNfsItalic
  NfsCliUfCod.FontItalic  = &CnfNfsItalic
  NfsNumPed.FontItalic    = &CnfNfsItalic
  NfsSts.FontItalic    = &CnfNfsItalic
  NfsBseClcIcms.FontItalic     = &CnfNfsItalic
  NfsVlrIcms.FontItalic        = &CnfNfsItalic
  NfsBseClcSt.FontItalic       = &CnfNfsItalic
  NfsVlrSt.FontItalic          = &CnfNfsItalic
  NfsVlrFrt.FontItalic         = &CnfNfsItalic
  NfsOutDsp.FontItalic      = &CnfNfsItalic
  NfsVlrDsc.FontItalic         = &CnfNfsItalic
  NfsBseClcIpi.FontItalic = &CnfNfsItalic
  NfsVlrIpi.FontItalic    = &CnfNfsItalic

  NfsNum.FontUnderline       = &CnfNfsUnderline
  NfsDtaEms.FontUnderline    = &CnfNfsUnderline
  NfsSer.FontUnderline       = &CnfNfsUnderline
  NfsCliNom.FontUnderline    = &CnfNfsUnderline
  NfsVlrTotNf.FontUnderline  = &CnfNfsUnderline
  NfsVlrTotPrd.FontUnderline = &CnfNfsUnderline
  NfsTipTrnDsc.FontUnderline     = &CnfNfsUnderline
  NfsVenNom.FontUnderline    = &CnfNfsUnderline
  NfsTrpNom.FontUnderline    = &CnfNfsUnderline
  NfsCliCidNom.FontUnderline = &CnfNfsUnderline
  NfsCliUfCod.FontUnderline  = &CnfNfsUnderline
  NfsNumPed.FontUnderline    = &CnfNfsUnderline
  NfsSts.FontUnderline    = &CnfNfsUnderline
  NfsBseClcIcms.FontUnderline    = &CnfNfsUnderline
  NfsVlrIcms.FontUnderline        = &CnfNfsUnderline
  NfsBseClcSt.FontUnderline    = &CnfNfsUnderline
  NfsVlrSt.FontUnderline         = &CnfNfsUnderline
  NfsVlrFrt.FontUnderline        = &CnfNfsUnderline
  NfsOutDsp.FontUnderline      = &CnfNfsUnderline
  NfsVlrDsc.FontUnderline         = &CnfNfsUnderline
  NfsBseClcIpi.FontUnderline = &CnfNfsUnderline
  NfsVlrIpi.FontUnderline    = &CnfNfsUnderline


  // Cores de Fundo
  do case
     case &CnfNfsCorLinha = 'BLK'

          NfsNum.BackColor       = rgb(0,0,0)
          NfsDtaEms.BackColor    = rgb(0,0,0)
          NfsSer.BackColor       = rgb(0,0,0)
          NfsCliNom.BackColor    = rgb(0,0,0)
          NfsVlrTotNf.BackColor  = rgb(0,0,0)
          NfsVlrTotPrd.BackColor = rgb(0,0,0)
          NfsTipTrnDsc.BackColor     = rgb(0,0,0)
          NfsVenNom.BackColor    = rgb(0,0,0)
          NfsTrpNom.BackColor    = rgb(0,0,0)
          NfsCliCidNom.BackColor = rgb(0,0,0)
          NfsCliUfCod.BackColor  = rgb(0,0,0)
          NfsNumPed.BackColor    = rgb(0,0,0)
          NfsSts.BackColor    = rgb(0,0,0)
          NfsBseClcIcms.BackColor       = rgb(0,0,0)
          NfsVlrIcms.BackColor       = rgb(0,0,0)
          NfsBseClcSt.BackColor       = rgb(0,0,0)
          NfsVlrSt.BackColor       = rgb(0,0,0)
          NfsVlrFrt.BackColor       = rgb(0,0,0)
          NfsOutDsp.BackColor       = rgb(0,0,0)
          NfsVlrDsc.BackColor       = rgb(0,0,0)
          NfsBseClcIpi.BackColor    = rgb(0,0,0)
          NfsVlrIpi.BackColor       = rgb(0,0,0)

     case &CnfNfsCorLinha = 'WHT'

          NfsNum.BackColor       = rgb(255,255,255)
          NfsDtaEms.BackColor    = rgb(255,255,255)
          NfsSer.BackColor       = rgb(255,255,255)
          NfsCliNom.BackColor    = rgb(255,255,255)
          NfsVlrTotNf.BackColor  = rgb(255,255,255)
          NfsVlrTotPrd.BackColor = rgb(255,255,255)
          NfsTipTrnDsc.BackColor     = rgb(255,255,255)
          NfsVenNom.BackColor    = rgb(255,255,255)
          NfsTrpNom.BackColor    = rgb(255,255,255)
          NfsCliCidNom.BackColor = rgb(255,255,255)
          NfsCliUfCod.BackColor  = rgb(255,255,255)
          NfsNumPed.BackColor    = rgb(255,255,255)
          NfsSts.BackColor    = rgb(255,255,255)
          NfsBseClcIcms.BackColor       = rgb(255,255,255)
          NfsVlrIcms.BackColor       = rgb(255,255,255)
          NfsBseClcSt.BackColor       = rgb(255,255,255)
          NfsVlrSt.BackColor       = rgb(255,255,255)
          NfsVlrFrt.BackColor       = rgb(255,255,255)
          NfsOutDsp.BackColor       = rgb(255,255,255)
          NfsVlrDsc.BackColor       = rgb(255,255,255)
          NfsBseClcIpi.BackColor    = rgb(255,255,255)
          NfsVlrIpi.BackColor       = rgb(255,255,255)

     case &CnfNfsCorLinha = 'YLW'

          NfsNum.BackColor       = rgb(255,255,0)
          NfsDtaEms.BackColor    = rgb(255,255,0)
          NfsSer.BackColor       = rgb(255,255,0)
          NfsCliNom.BackColor    = rgb(255,255,0)
          NfsVlrTotNf.BackColor  = rgb(255,255,0)
          NfsVlrTotPrd.BackColor = rgb(255,255,0)
          NfsTipTrnDsc.BackColor     = rgb(255,255,0)
          NfsVenNom.BackColor    = rgb(255,255,0)
          NfsTrpNom.BackColor    = rgb(255,255,0)
          NfsCliCidNom.BackColor = rgb(255,255,0)
          NfsCliUfCod.BackColor  = rgb(255,255,0)
          NfsNumPed.BackColor    = rgb(255,255,0)
          NfsSts.BackColor    = rgb(255,255,0)
          NfsBseClcIcms.BackColor       = rgb(255,255,0)
          NfsVlrIcms.BackColor       = rgb(255,255,0)
          NfsBseClcSt.BackColor       = rgb(255,255,0)
          NfsVlrSt.BackColor       = rgb(255,255,0)
          NfsVlrFrt.BackColor       = rgb(255,255,0)
          NfsOutDsp.BackColor       = rgb(255,255,0)
          NfsVlrDsc.BackColor       = rgb(255,255,0)
          NfsBseClcIpi.BackColor    = rgb(255,255,0)
          NfsVlrIpi.BackColor       = rgb(255,255,0)

     case &CnfNfsCorLinha = 'BLU'

          NfsNum.BackColor       = rgb(0,0,255)
          NfsDtaEms.BackColor    = rgb(0,0,255)
          NfsSer.BackColor       = rgb(0,0,255)
          NfsCliNom.BackColor    = rgb(0,0,255)
          NfsVlrTotNf.BackColor  = rgb(0,0,255)
          NfsVlrTotPrd.BackColor = rgb(0,0,255)
          NfsTipTrnDsc.BackColor     = rgb(0,0,255)
          NfsVenNom.BackColor    = rgb(0,0,255)
          NfsTrpNom.BackColor    = rgb(0,0,255)
          NfsCliCidNom.BackColor = rgb(0,0,255)
          NfsCliUfCod.BackColor  = rgb(0,0,255)
          NfsNumPed.BackColor    = rgb(0,0,255)
          NfsSts.BackColor    = rgb(0,0,255)
          NfsBseClcIcms.BackColor       = rgb(0,0,255)
          NfsVlrIcms.BackColor       = rgb(0,0,255)
          NfsBseClcSt.BackColor       = rgb(0,0,255)
          NfsVlrSt.BackColor       = rgb(0,0,255)
          NfsVlrFrt.BackColor       = rgb(0,0,255)
          NfsVlrDsc.BackColor       = rgb(0,0,255)
          NfsOutDsp.BackColor       = rgb(0,0,255)
          NfsBseClcIpi.BackColor    = rgb(0,0,255)
          NfsVlrIpi.BackColor       = rgb(0,0,255)

     case &CnfNfsCorLinha = 'RED'

          NfsNum.BackColor       = rgb(255,0,0)
          NfsDtaEms.BackColor    = rgb(255,0,0)
          NfsSer.BackColor       = rgb(255,0,0)
          NfsCliNom.BackColor    = rgb(255,0,0)
          NfsVlrTotNf.BackColor  = rgb(255,0,0)
          NfsVlrTotPrd.BackColor = rgb(255,0,0)
          NfsTipTrnDsc.BackColor     = rgb(255,0,0)
          NfsVenNom.BackColor    = rgb(255,0,0)
          NfsTrpNom.BackColor    = rgb(255,0,0)
          NfsCliCidNom.BackColor = rgb(255,0,0)
          NfsCliUfCod.BackColor  = rgb(255,0,0)
          NfsNumPed.BackColor    = rgb(255,0,0)
          NfsSts.BackColor    = rgb(255,0,0)
          NfsBseClcIcms.BackColor       = rgb(255,0,0)
          NfsVlrIcms.BackColor       = rgb(255,0,0)
          NfsBseClcSt.BackColor       = rgb(255,0,0)
          NfsVlrSt.BackColor       = rgb(255,0,0)
          NfsVlrFrt.BackColor       = rgb(255,0,0)
          NfsVlrDsc.BackColor       = rgb(255,0,0)
          NfsOutDsp.BackColor       = rgb(255,0,0)
          NfsBseClcIpi.BackColor    = rgb(255,0,0)
          NfsVlrIpi.BackColor       = rgb(255,0,0)

     case &CnfNfsCorLinha = 'CYN'

          NfsNum.BackColor       = rgb(0,255,255)
          NfsDtaEms.BackColor    = rgb(0,255,255)
          NfsSer.BackColor       = rgb(0,255,255)
          NfsCliNom.BackColor    = rgb(0,255,255)
          NfsVlrTotNf.BackColor  = rgb(0,255,255)
          NfsVlrTotPrd.BackColor = rgb(0,255,255)
          NfsTipTrnDsc.BackColor     = rgb(0,255,255)
          NfsVenNom.BackColor    = rgb(0,255,255)
          NfsTrpNom.BackColor    = rgb(0,255,255)
          NfsCliCidNom.BackColor = rgb(0,255,255)
          NfsCliUfCod.BackColor  = rgb(0,255,255)
          NfsNumPed.BackColor    = rgb(0,255,255)
          NfsSts.BackColor    = rgb(0,255,255)
          NfsBseClcIcms.BackColor       = rgb(0,255,255)
          NfsVlrIcms.BackColor       = rgb(0,255,255)
          NfsBseClcSt.BackColor       = rgb(0,255,255)
          NfsVlrSt.BackColor       = rgb(0,255,255)
          NfsVlrFrt.BackColor       = rgb(0,255,255)
          NfsVlrDsc.BackColor       = rgb(0,255,255)
          NfsOutDsp.BackColor       = rgb(0,255,255)
          NfsBseClcIpi.BackColor    = rgb(0,255,255)
          NfsVlrIpi.BackColor       = rgb(0,255,255)

     case &CnfNfsCorLinha = 'MGN'

          NfsNum.BackColor       = rgb(255,0,255)
          NfsDtaEms.BackColor    = rgb(255,0,255)
          NfsSer.BackColor       = rgb(255,0,255)
          NfsCliNom.BackColor    = rgb(255,0,255)
          NfsVlrTotNf.BackColor  = rgb(255,0,255)
          NfsVlrTotPrd.BackColor = rgb(255,0,255)
          NfsTipTrnDsc.BackColor     = rgb(255,0,255)
          NfsVenNom.BackColor    = rgb(255,0,255)
          NfsTrpNom.BackColor    = rgb(255,0,255)
          NfsCliCidNom.BackColor = rgb(255,0,255)
          NfsCliUfCod.BackColor  = rgb(255,0,255)
          NfsNumPed.BackColor    = rgb(255,0,255)
          NfsSts.BackColor    = rgb(255,0,255)
          NfsBseClcIcms.BackColor       = rgb(255,0,255)
          NfsVlrIcms.BackColor       = rgb(255,0,255)
          NfsBseClcSt.BackColor       = rgb(255,0,255)
          NfsVlrSt.BackColor       = rgb(255,0,255)
          NfsVlrFrt.BackColor       = rgb(255,0,255)
          NfsVlrDsc.BackColor       = rgb(255,0,255)
          NfsOutDsp.BackColor       = rgb(255,0,255)
          NfsBseClcIpi.BackColor    = rgb(255,0,255)
          NfsVlrIpi.BackColor       = rgb(255,0,255)

     case &CnfNfsCorLinha = 'GRN' 

          NfsNum.BackColor       = rgb(0,255,0)
          NfsDtaEms.BackColor    = rgb(0,255,0)
          NfsSer.BackColor       = rgb(0,255,0)
          NfsCliNom.BackColor    = rgb(0,255,0)
          NfsVlrTotNf.BackColor  = rgb(0,255,0)
          NfsVlrTotPrd.BackColor = rgb(0,255,0)
          NfsTipTrnDsc.BackColor     = rgb(0,255,0)
          NfsVenNom.BackColor    = rgb(0,255,0)
          NfsTrpNom.BackColor    = rgb(0,255,0)
          NfsCliCidNom.BackColor = rgb(0,255,0)
          NfsCliUfCod.BackColor  = rgb(0,255,0)
          NfsNumPed.BackColor    = rgb(0,255,0)
          NfsSts.BackColor    = rgb(0,255,0)
          NfsBseClcIcms.BackColor       = rgb(0,255,0)
          NfsVlrIcms.BackColor       = rgb(0,255,0)
          NfsBseClcSt.BackColor       = rgb(0,255,0)
          NfsVlrSt.BackColor       = rgb(0,255,0)
          NfsVlrFrt.BackColor       = rgb(0,255,0)
          NfsVlrDsc.BackColor       = rgb(0,255,0)
          NfsOutDsp.BackColor       = rgb(0,255,0)
          NfsBseClcIpi.BackColor    = rgb(0,255,0)
          NfsVlrIpi.BackColor       = rgb(0,255,0)

     case &CnfNfsCorLinha = 'BRW'

          NfsNum.BackColor       = rgb(165,42,42)
          NfsDtaEms.BackColor    = rgb(165,42,42)
          NfsSer.BackColor       = rgb(165,42,42)
          NfsCliNom.BackColor    = rgb(165,42,42)
          NfsVlrTotNf.BackColor  = rgb(165,42,42)
          NfsVlrTotPrd.BackColor = rgb(165,42,42)
          NfsTipTrnDsc.BackColor     = rgb(165,42,42)
          NfsVenNom.BackColor    = rgb(165,42,42)
          NfsTrpNom.BackColor    = rgb(165,42,42)
          NfsCliCidNom.BackColor = rgb(165,42,42)
          NfsCliUfCod.BackColor  = rgb(165,42,42)
          NfsNumPed.BackColor    = rgb(165,42,42)
          NfsSts.BackColor    = rgb(165,42,42)
          NfsBseClcIcms.BackColor       = rgb(165,42,42)
          NfsVlrIcms.BackColor       = rgb(165,42,42)
          NfsBseClcSt.BackColor       = rgb(165,42,42)
          NfsVlrSt.BackColor       = rgb(165,42,42)
          NfsVlrFrt.BackColor       = rgb(165,42,42)
          NfsVlrDsc.BackColor       = rgb(165,42,42)
          NfsOutDsp.BackColor       = rgb(165,42,42)
          NfsBseClcIpi.BackColor    = rgb(165,42,42)
          NfsVlrIpi.BackColor       = rgb(165,42,42)

  endcase



  do case
     case &CnfNfsCorFonte = 'BLK'

          NfsNum.ForeColor       = rgb(0,0,0)
          NfsDtaEms.ForeColor    = rgb(0,0,0)
          NfsSer.ForeColor       = rgb(0,0,0)
          NfsCliNom.ForeColor    = rgb(0,0,0)
          NfsVlrTotNf.ForeColor  = rgb(0,0,0)
          NfsVlrTotPrd.ForeColor = rgb(0,0,0)
          NfsTipTrnDsc.ForeColor     = rgb(0,0,0)
          NfsVenNom.ForeColor    = rgb(0,0,0)
          NfsTrpNom.ForeColor    = rgb(0,0,0)
          NfsCliCidNom.ForeColor = rgb(0,0,0)
          NfsCliUfCod.ForeColor  = rgb(0,0,0)
          NfsNumPed.ForeColor    = rgb(0,0,0)
          NfsSts.ForeColor    = rgb(0,0,0)
          NfsBseClcIcms.ForeColor    = rgb(0,0,0)
          NfsVlrIcms.ForeColor    = rgb(0,0,0)
          NfsBseClcSt.ForeColor    = rgb(0,0,0)
          NfsVlrSt.ForeColor    = rgb(0,0,0)
          NfsVlrFrt.ForeColor    = rgb(0,0,0)
          NfsVlrDsc.ForeColor    = rgb(0,0,0)
          NfsOutDsp.ForeColor    = rgb(0,0,0)
          NfsBseClcIpi.ForeColor    = rgb(0,0,0)
          NfsVlrIpi.ForeColor       = rgb(0,0,0)

     case &CnfNfsCorFonte = 'WHT'

          NfsNum.ForeColor       = rgb(255,255,255)
          NfsDtaEms.ForeColor    = rgb(255,255,255)
          NfsSer.ForeColor       = rgb(255,255,255)
          NfsCliNom.ForeColor    = rgb(255,255,255)
          NfsVlrTotNf.ForeColor  = rgb(255,255,255)
          NfsVlrTotPrd.ForeColor = rgb(255,255,255)
          NfsTipTrnDsc.ForeColor     = rgb(255,255,255)
          NfsVenNom.ForeColor    = rgb(255,255,255)
          NfsTrpNom.ForeColor    = rgb(255,255,255)
          NfsCliCidNom.ForeColor = rgb(255,255,255)
          NfsCliUfCod.ForeColor  = rgb(255,255,255)
          NfsNumPed.ForeColor    = rgb(255,255,255)
          NfsSts.ForeColor    = rgb(255,255,255)
          NfsBseClcIcms.ForeColor    = rgb(255,255,255)
          NfsVlrIcms.ForeColor    = rgb(255,255,255)
          NfsBseClcSt.ForeColor    = rgb(255,255,255)
          NfsVlrSt.ForeColor    = rgb(255,255,255)
          NfsVlrFrt.ForeColor    = rgb(255,255,255)
          NfsVlrDsc.ForeColor    = rgb(255,255,255)
          NfsOutDsp.ForeColor    = rgb(255,255,255)
          NfsBseClcIpi.ForeColor    = rgb(255,255,255)
          NfsVlrIpi.ForeColor       = rgb(255,255,255)

     case &CnfNfsCorFonte = 'YLW'

          NfsNum.ForeColor       = rgb(255,255,0)
          NfsDtaEms.ForeColor    = rgb(255,255,0)
          NfsSer.ForeColor       = rgb(255,255,0)
          NfsCliNom.ForeColor    = rgb(255,255,0)
          NfsVlrTotNf.ForeColor  = rgb(255,255,0)
          NfsVlrTotPrd.ForeColor = rgb(255,255,0)
          NfsTipTrnDsc.ForeColor     = rgb(255,255,0)
          NfsVenNom.ForeColor    = rgb(255,255,0)
          NfsTrpNom.ForeColor    = rgb(255,255,0)
          NfsCliCidNom.ForeColor = rgb(255,255,0)
          NfsCliUfCod.ForeColor  = rgb(255,255,0)
          NfsNumPed.ForeColor    = rgb(255,255,0)
          NfsSts.ForeColor    = rgb(255,255,0)
          NfsBseClcIcms.ForeColor    = rgb(255,255,0)
          NfsVlrIcms.ForeColor    = rgb(255,255,0)
          NfsBseClcSt.ForeColor    = rgb(255,255,0)
          NfsVlrSt.ForeColor    = rgb(255,255,0)
          NfsVlrFrt.ForeColor    = rgb(255,255,0)
          NfsVlrDsc.ForeColor    = rgb(255,255,0)
          NfsOutDsp.ForeColor    = rgb(255,255,0)
          NfsBseClcIpi.ForeColor    = rgb(255,255,0)
          NfsVlrIpi.ForeColor       = rgb(255,255,0)

     case &CnfNfsCorFonte = 'BLU'

          NfsNum.ForeColor       = rgb(0,0,255)
          NfsDtaEms.ForeColor    = rgb(0,0,255)
          NfsSer.ForeColor       = rgb(0,0,255)
          NfsCliNom.ForeColor    = rgb(0,0,255)
          NfsVlrTotNf.ForeColor  = rgb(0,0,255)
          NfsVlrTotPrd.ForeColor = rgb(0,0,255)
          NfsTipTrnDsc.ForeColor     = rgb(0,0,255)
          NfsVenNom.ForeColor    = rgb(0,0,255)
          NfsTrpNom.ForeColor    = rgb(0,0,255)
          NfsCliCidNom.ForeColor = rgb(0,0,255)
          NfsCliUfCod.ForeColor  = rgb(0,0,255)
          NfsNumPed.ForeColor    = rgb(0,0,255)
          NfsSts.ForeColor    = rgb(0,0,255)
          NfsBseClcIcms.ForeColor    = rgb(0,0,255)
          NfsVlrIcms.ForeColor    = rgb(0,0,255)
          NfsBseClcSt.ForeColor    = rgb(0,0,255)
          NfsVlrSt.ForeColor    = rgb(0,0,255)
          NfsVlrFrt.ForeColor    = rgb(0,0,255)
          NfsVlrDsc.ForeColor    = rgb(0,0,255)
          NfsOutDsp.ForeColor    = rgb(0,0,255)
          NfsBseClcIpi.ForeColor    = rgb(0,0,255)
          NfsVlrIpi.ForeColor       = rgb(0,0,255)

     case &CnfNfsCorFonte = 'RED'

          NfsNum.ForeColor       = rgb(255,0,0)
          NfsDtaEms.ForeColor    = rgb(255,0,0)
          NfsSer.ForeColor       = rgb(255,0,0)
          NfsCliNom.ForeColor    = rgb(255,0,0)
          NfsVlrTotNf.ForeColor  = rgb(255,0,0)
          NfsVlrTotPrd.ForeColor = rgb(255,0,0)
          NfsTipTrnDsc.ForeColor     = rgb(255,0,0)
          NfsVenNom.ForeColor    = rgb(255,0,0)
          NfsTrpNom.ForeColor    = rgb(255,0,0)
          NfsCliCidNom.ForeColor = rgb(255,0,0)
          NfsCliUfCod.ForeColor  = rgb(255,0,0)
          NfsNumPed.ForeColor    = rgb(255,0,0)
          NfsSts.ForeColor    = rgb(255,0,0)
          NfsBseClcIcms.ForeColor    = rgb(255,0,0)
          NfsVlrIcms.ForeColor    = rgb(255,0,0)
          NfsBseClcSt.ForeColor    = rgb(255,0,0)
          NfsVlrSt.ForeColor    = rgb(255,0,0)
          NfsVlrFrt.ForeColor    = rgb(255,0,0)
          NfsVlrDsc.ForeColor    = rgb(255,0,0)
          NfsOutDsp.ForeColor    = rgb(255,0,0)
          NfsBseClcIpi.ForeColor    = rgb(255,0,0)
          NfsVlrIpi.ForeColor       = rgb(255,0,0)

     case &CnfNfsCorFonte = 'CYN'

          NfsNum.ForeColor       = rgb(0,255,255)
          NfsDtaEms.ForeColor    = rgb(0,255,255)
          NfsSer.ForeColor       = rgb(0,255,255)
          NfsCliNom.ForeColor    = rgb(0,255,255)
          NfsVlrTotNf.ForeColor  = rgb(0,255,255)
          NfsVlrTotPrd.ForeColor = rgb(0,255,255)
          NfsTipTrnDsc.ForeColor     = rgb(0,255,255)
          NfsVenNom.ForeColor    = rgb(0,255,255)
          NfsTrpNom.ForeColor    = rgb(0,255,255)
          NfsCliCidNom.ForeColor = rgb(0,255,255)
          NfsCliUfCod.ForeColor  = rgb(0,255,255)
          NfsNumPed.ForeColor    = rgb(0,255,255)
          NfsSts.ForeColor    = rgb(0,255,255)
          NfsBseClcIcms.ForeColor    = rgb(0,255,255)
          NfsVlrIcms.ForeColor    = rgb(0,255,255)
          NfsBseClcSt.ForeColor    = rgb(0,255,255)
          NfsVlrSt.ForeColor    = rgb(0,255,255)
          NfsVlrFrt.ForeColor    = rgb(0,255,255)
          NfsVlrDsc.ForeColor    = rgb(0,255,255)
          NfsOutDsp.ForeColor    = rgb(0,255,255)
          NfsBseClcIpi.ForeColor    = rgb(0,255,255)
          NfsVlrIpi.ForeColor       = rgb(0,255,255)

     case &CnfNfsCorFonte = 'MGN'

          NfsNum.ForeColor       = rgb(255,0,255)
          NfsDtaEms.ForeColor    = rgb(255,0,255)
          NfsSer.ForeColor       = rgb(255,0,255)
          NfsCliNom.ForeColor    = rgb(255,0,255)
          NfsVlrTotNf.ForeColor  = rgb(255,0,255)
          NfsVlrTotPrd.ForeColor = rgb(255,0,255)
          NfsTipTrnDsc.ForeColor     = rgb(255,0,255)
          NfsVenNom.ForeColor    = rgb(255,0,255)
          NfsTrpNom.ForeColor    = rgb(255,0,255)
          NfsCliCidNom.ForeColor = rgb(255,0,255)
          NfsCliUfCod.ForeColor  = rgb(255,0,255)
          NfsNumPed.ForeColor    = rgb(255,0,255)
          NfsSts.ForeColor    = rgb(255,0,255)
          NfsBseClcIcms.ForeColor    = rgb(255,0,255)
          NfsVlrIcms.ForeColor    = rgb(255,0,255)
          NfsBseClcSt.ForeColor    = rgb(255,0,255)
          NfsVlrSt.ForeColor    = rgb(255,0,255)
          NfsVlrFrt.ForeColor    = rgb(255,0,255)
          NfsVlrDsc.ForeColor    = rgb(255,0,255)
          NfsOutDsp.ForeColor    = rgb(255,0,255)
          NfsBseClcIpi.ForeColor    = rgb(255,0,255)
          NfsVlrIpi.ForeColor       = rgb(255,0,255)

     case &CnfNfsCorFonte = 'GRN' 

          NfsNum.ForeColor       = rgb(0,255,0)
          NfsDtaEms.ForeColor    = rgb(0,255,0)
          NfsSer.ForeColor       = rgb(0,255,0)
          NfsCliNom.ForeColor    = rgb(0,255,0)
          NfsVlrTotNf.ForeColor  = rgb(0,255,0)
          NfsVlrTotPrd.ForeColor = rgb(0,255,0)
          NfsTipTrnDsc.ForeColor     = rgb(0,255,0)
          NfsVenNom.ForeColor    = rgb(0,255,0)
          NfsTrpNom.ForeColor    = rgb(0,255,0)
          NfsCliCidNom.ForeColor = rgb(0,255,0)
          NfsCliUfCod.ForeColor  = rgb(0,255,0)
          NfsNumPed.ForeColor    = rgb(0,255,0)
          NfsSts.ForeColor    = rgb(0,255,0)
          NfsBseClcIcms.ForeColor    = rgb(0,255,0)
          NfsVlrIcms.ForeColor    = rgb(0,255,0)
          NfsBseClcSt.ForeColor    = rgb(0,255,0)
          NfsVlrSt.ForeColor    = rgb(0,255,0)
          NfsVlrFrt.ForeColor    = rgb(0,255,0)
          NfsVlrDsc.ForeColor    = rgb(0,255,0)
          NfsOutDsp.ForeColor    = rgb(0,255,0)
          NfsBseClcIpi.ForeColor    = rgb(0,255,0)
          NfsVlrIpi.ForeColor       = rgb(0,255,0)

     case &CnfNfsCorFonte = 'BRW'

          NfsNum.ForeColor       = rgb(165,42,42)
          NfsDtaEms.ForeColor    = rgb(165,42,42)
          NfsSer.ForeColor       = rgb(165,42,42)
          NfsCliNom.ForeColor    = rgb(165,42,42)
          NfsVlrTotNf.ForeColor  = rgb(165,42,42)
          NfsVlrTotPrd.ForeColor = rgb(165,42,42)
          NfsTipTrnDsc.ForeColor     = rgb(165,42,42)
          NfsVenNom.ForeColor    = rgb(165,42,42)
          NfsTrpNom.ForeColor    = rgb(165,42,42)
          NfsCliCidNom.ForeColor = rgb(165,42,42)
          NfsCliUfCod.ForeColor  = rgb(165,42,42)
          NfsNumPed.ForeColor    = rgb(165,42,42)
          NfsSts.ForeColor    = rgb(165,42,42)
          NfsBseClcIcms.ForeColor    = rgb(165,42,42)
          NfsVlrIcms.ForeColor    = rgb(165,42,42)
          NfsBseClcSt.ForeColor    = rgb(165,42,42)
          NfsVlrSt.ForeColor    = rgb(165,42,42)
          NfsVlrFrt.ForeColor    = rgb(165,42,42)
          NfsVlrDsc.ForeColor    = rgb(165,42,42)
          NfsOutDsp.ForeColor    = rgb(165,42,42)
          NfsBseClcIpi.ForeColor    = rgb(165,42,42)
          NfsVlrIpi.ForeColor       = rgb(165,42,42)

  endcase
endsub

Event 'Cancelar'
    if not null(NfsNum)
        If NfsSts = 'E'
            &Mensagem = 'Deseja realmente CANCELAR a nota fiscal nº. ' + trim(str(NfsNum)) + ' ?'
            Confirm(&Mensagem, N)
            If Confirmed()
                Call(PCancelaNF, &Logon,NfsNum,NfsSer,NfsNumPed)
                refresh keep
            endif
        Else
            Msg('Essa NF não pode ser Cancelada pois não está com status Enviada!')
        EndIf
    Else
        Msg('Favor selecionar uma NF para Cancelar!')
    endif
EndEvent  // 'Cancelar'


Event 'nfe'
&SdtNota.Clear()
if not null(NfsNum)

  confirm('Deseja Gerar o arquivo para a Nota Fiscal Eletrônica ?')
  if confirmed()
      for each line 
         Call(PVerStsNF, &Logon, NfsNum, NfsSer, &NfsSts2)
         if &Esc = 'S' and &NfsSts2 <> 'C'
    
               &SdtNotaItem = new SdtNota.SdtNotaItem()
               &SdtNotaItem.NfsNum = NfsNum
               &SdtNotaItem.NfsSer = NfsSer
               &SdtNota.Add(&SdtNotaItem)
   
    
         endif
      endfor
  endif

  if &SdtNota.Count > 0
    
    if null(&EmpDirTxtNfe)
        csharp string dirAtual = System.Windows.Forms.Application.StartupPath;
        csharp [!&diretorio!] = dirAtual;
    
        &diretorio += '\TXT NFE'
    
        call('gxSelDir',&Arquivo,&diretorio,'Selecione o diretório',0)


         if not null(&arquivo)

             call(PNfe4,&Logon,&SdtNota,&arquivo)

             Msg('Gerado com sucesso !!!')

        endif

    else
         &arquivo = &EmpDirTxtNfe

         call(PNfe4,&Logon,&SdtNota,&arquivo)
  
         msg('Arquivo Gerado no Diretório: '+&arquivo.Trim())

    endif

    REFRESH

     
  else
      msg('Selecione ao menos uma nota fiscal')
  endif

endif
EndEvent  // 'nfe'

Event 'Dsp'
if not null(NfsNum)

     &LOGHST = 'NOTA FISCAL DE VENDA: '+TRIM(STR(NfsNum))+' - '+TRIM(NfsSer)
     PLogOperacao.Call(&logon.UsrCod,'DSP',&LOGHST)

     call(WNotaFiscal3,&Logon,NfsNum,NfsSer)

endif
EndEvent  // 'Dsp'

Event 'DANFE'
call(RDanfe,NfsNum,NfsSer)
EndEvent  // 'DANFE'


sub'tipo'
for each
   where TpEndSeq = &EmpTpEndSeq
   &TipoEnd = TpEndDsc.Trim()
endfor
endsub
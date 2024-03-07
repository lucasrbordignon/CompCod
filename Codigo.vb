event start

    Form.Caption = Form.Caption + ' - (' + Trim(&Pgmname) + ')'

   // btnalt.TooltipText = 'Alterar Registro' // Retirado dia 29/12/2017 pois a parte fiscal foi refeita, agora pode-se alterar a CFOP diretamente no pedido caso necessário.
   btndsp.TooltipText = 'Visualizar o Registro'

   &empcod = &SdtNf.NfsEmpCod
   &PedCod = &SdtNf.NfsNumPed

   &nfsvlricms = &SdtNf.NfsVlrIcms
   &NfsVlrSt   = &SdtNf.NfsVlrSt
   &NfsVlrDsc  = &SdtNf.NfsVlrDsc
   &NfsBseClcIcms = &SdtNf.NfsBseClcIcms

   &NfsBseClcSt = &SdtNf.NfsBseClcSt
   &NfsVlrFrt = &SdtNf.NfsVlrFrt
   &NfsVlrTotPrd = &SdtNf.NfsVlrTotPrd
   &NfsVlrIpi = &SdtNf.NfsVlrIpi
   &NfsVlrTotNf = &SdtNf.NfsVlrTotNf
   &NfsVlrPis   = &SdtNf.nfsvlrpis
   &NfsVlrCofins = &SdtNf.NfsVlrCofins
   &NfsCliCod = &SdtNf.NfsCliCod
//   If &Logon.EmpClienteNa12 = 6  //CANCIAN
//       &NfsEsp = 'CAIXA'
//       &NfsQtd =  &SdtNf.PedQtdeCaixa       
//   else
       &NfsQtd    = &SdtNf.NfsQtd
 //  endif
   &NfsInfCmp = &SdtNf.NfsInfCmp
   &NfsPesoBruto = &SdtNf.NfsPesoBruto
   &NfsPesoLiquido = &SdtNf.NfsPesoliquido
   &NfsOutDsp = &SdtNf.NfsOutDsp
   &NfsVlrDsc = &SdtNf.NfsVlrDsc
   &CpgPla1Cod = &SdtNf.NfsPla1Cod
   &CpgPla2Cod = &SdtNf.NfsPla2Cod
   &CpgPla3Cod = &SdtNf.NfsPla3Cod
   &CpgPla4Cod = &SdtNf.NfsPla4Cod
   &CpgPla5Cod = &SdtNf.NfsPla5Cod
   &NfsVlrIcmsDest = &SdtNf.NfsVlrIcmsDest
   &NfsVlrIcmsOri = &SdtNf.NfsVlrIcmsOri
   &NfsVlrFCP = &SdtNf.NfsVlrFCP
   &NfsDesIcms = &SdtNf.NfsDesIcms
   &NfsVlrIPIDev = &SdtNf.NfsVlrIPIDev
   &NfsVlrFCPSub = &SdtNf.NfsVlrFCPSub
   &NfsTpTrn = &SdtNf.NfsTpTrn
   &NfsTipTrnNOP = &SdtNf.NfsTipTrnNOP

   for each
    where Pla1Cod = &CpgPla1Cod
    where Pla2Cod = &CpgPla2Cod
    where Pla3Cod = &CpgPla3Cod
    where Pla4Cod = &CpgPla4Cod
    Where Pla5Cod = &CpgPla5cOD

    &Pla5CodDesc = Pla5CodDesc
    &CpgPla4Dsc = Pla5Dsc

   endfor

   for each
       where TrpAtv = 1

       &NfsTrpCod.Additem(TrpCod,TrpNom)

   endfor

   &NfsUfEmb.Additem('','')
   &NfsUfEmb = ''

   for each
      where EmpCod = &empcod

      &EmpNom = EmpNom
      &Bit  = loadbitmap(EmpLogoNome)
      &EmpTpEndSeq = EmpTpEndSeq
      do'tipo'
      &End1  = &TpEndDsc+space(1)+trim(EmpEnd)+','+trim(EmpEndNum)+'-'+trim(EmpEndBai)
      &end2  = trim(EmpCidade)+','+trim(EmpUf)+'- CEP:'+TRIM(EmpCep)
      &End3 = 'Fone/Fax:'+trim(EmpFone)
      &EmpIe = EmpIE

      if EmpIEise = 'S'
         &EmpIe = 'ISENTO'
      endif

      &EmpCnpj = EmpCnpj
      &EmpUf = EmpUf
      &EmpUsaConvenio = EmpUsaConvenio
      &EmpCRT = EmpCRT

   endfor

   for each
      where UfCod = &EmpUf

      &UfInscSub = UfInscSub

   endfor

   &nota2 = &SdtNf.NfsNum.ToString()
   &nota2 = padl(trim(&nota2),11,'0')

   &NfsSer = Padl(trim(&SdtNf.NfsSer),3,'0')
   &NfsTpNf = &SdtNf.NfsTpNf
   &NfsDtaEms = &SdtNf.NfsDtaEms
   &NfsDtaSai = &SdtNf.NfsDtaSai
   &NfsHraSai = &SdtNf.NfsHraSai
   &NfsCubagem = &SdtNf.NfsCubagem
   &NfsTrpCod = &SdtNf.NfsTrpCod
    
   //Importação
   for each
     where NfsNum = &SdtNf.NfsNum
     where NfsSer = &SdtNf.NfsSer
     defined by NfiImpNroDI
        
       &SdtImpItem = new SdtImp.SdtImpItem()
       &SdtImpItem.NfiImpNroDI        = NfiImpNroDI
       &SdtImpItem.NfiImpDtaDI        = NfiImpDtaDI
       &SdtImpItem.NfiImpcExportador  = NfiImpcExportador
       &SdtImpItem.NfiImpViaTransp    = NfiImpViaTransp
       &SdtImpItem.NfiImpVlrAFRMM     = NfiImpVlrAFRMM
       &SdtImpItem.NfiImpTpIntermedio = NfiImpTpIntermedio
       &SdtImpItem.NfiImpUFDesemb     = NfiImpUFDesemb
       &SdtImpItem.NfiImpLocDesemb    = NfiImpLocDesemb
       &SdtImpItem.NfiImpDtaDesemb    = NfiImpDtaDesemb
       &SdtImpItem.NfiImpCNPJ         = NfiImpCNPJ
       &SdtImpItem.NfiImpUFTer        = NfiImpUFTer
       &SdtImpItem.NfiImpSeq          = NfiImpSeq
       &SdtImpItem.NfiSeq             = NfiSeq
       &SdtImp.Add(&SdtImpItem)
   endfor
   
   // Adições
   for each
     where NfsNum = &SdtNf.NfsNum
     where NfsSer = &SdtNf.NfsSer
     defined by NfiImpAdcSeq
      
       &SdtADIItem = new SdtAdi.SdtAdiItem()
       &SdtADIItem.NfiImpAdcNro     = NfiImpAdcNro
       &SdtADIItem.NfiImpAdcFab     = NfiImpAdcFab
       &SdtADIItem.NfiImpAdcNroDraw = NfiImpAdcNroDraw
       &SdtADIItem.NfiImpAdcVlrDesc = NfiImpAdcVlrDesc
       &SdtADIItem.NfiImpAdcSeq     = NfiImpAdcSeq
       &SdtADIItem.NfiSeq           = NfiSeq
       &SdtADIItem.NfiImpSeq        = NfiImpSeq
       &SdtADI.Add(&SdtADIItem)
     
   endfor

   for each
     where VenCod = &SdtNf.NfsVenCod

     &VenNom = VenNom

   endfor

   for each
      where CliCod = &SdtNf.NfsCliCod

      &NfsCliNom = CliNom
      &NfsCliTp  = CliTp

      if CliTp = 'J'
         &NfsCliCpf.Visible  = 0
         &NfsCliCnpj.Visible = 1
         &NfsCliCnpj = CliCnpj
      else
         &NfsCliCpf.Visible  = 1
         &NfsCliCnpj.Visible = 0
         &NfsCliCpf = CliCpf
      endif

      &NfsCliEnd = CliEnd
      &NfsCliBai = CliEndBai
      &NfsCliCep = CliCep
      &NfsCliCidNom = CliCidNom
      &NfsCliFone = CliFone
      &NfsCliEstado = CliUfCod
      &NfsCliIes = CliIes
      &CliConvCod = CliConvCod

  endfor

  if &EmpUsaConvenio = 1
  
       for each
           where ConvCod = &CliConvCod
    
            &ConvDiaFec = ConvDiaFec
    
       endfor
    
  else
       &ConvDiaFec = 0
  endif


  &NfsTpFrt = &SdtNf.NfsTpFrt


  do case 
     case trim(&NfsCliEstado) = trim(&EmpUf) and &NfsTpNf = 1
     &char = '5'
     case trim(&NfsCliEstado) <> trim(&EmpUf) and trim(&NfsCliEstado) <> 'EX' and &NfsTpNf = 1
     &char = '6'
     case trim(&NfsCliEstado) = 'EX' and &NfsTpNf = 1
     &char = '7'
     case trim(&NfsCliEstado) = 'EX' and &NfsTpNf = 0
     &char = '3'
     case trim(&NfsCliEstado) = trim(&EmpUf) and &NfsTpNf = 0
     &char = '1'
     case trim(&NfsCliEstado) <> trim(&EmpUf) and trim(&NfsCliEstado) <> 'EX' and &NfsTpNf = 0
     &char = '2'
  endcase

  &NfsDescCfop = &SdtNf.NfsDescCfop


  for &SdtNfProItem in &SdtNfPro
    
      &NfsCfopSeq = &SdtNfProItem.NfiCfopSeq
      exit  

  endfor

   if &NfsTpFrt = 9
       &NfsTrpCod.Enabled = 0
       &NfsAntt.Enabled = 0
       &NfsPlcVei.Enabled = 0
       &NfsVeiUf.Enabled = 0
       &NfsForCpf = ''
       &NfsForCnpj = ''
       &NfsForEnd = ''
       &NfsForCidNom = ''
       &NfsForEstado = ''
       &NfsForIes = ''
       &NfsTrpCod = 0
       &NfsAntt = ''
       &NfsPlcVei = ''
       &NfsVeiUf = ''
   else
       if not null(&NfsTrpCod)

           for each
              where TrpCod = &NfsTrpCod
            
              if TrpTp = 'J'
                &NfsForCnpj = TrpCnpj
                &NfsForCpf = ''
                &NfsForCnpj.Visible = 1
                &NfsForCpf.Visible = 0
              else
                &NfsForCpf = TrpCpf
                &NfsForCnpj = ''
                &NfsForCnpj.Visible = 0
                &NfsForCpf.Visible = 1
              endif
            
              &NfsForEnd = TrpEnd.Trim()+','+TrpEndNum.Trim()
              &NfsForCidNom = TrpCidNom
              &NfsForEstado = TrpUFCod
              &NfsForIes = TrpIe
              &NfsPlcVei = TrpPlaca
              &NfsAntt = TrpAntt
              &NfsVeiUf = TrpPlacaUF
              when none
            
              &NfsForCpf = ''
              &NfsForCnpj = ''
              &NfsForEnd = ''
              &NfsForCidNom = ''
              &NfsForEstado = ''
              &NfsForIes = ''
              &NfsTrpCod = 0
            
            endfor

         endif
   endif

   if &EmpCrt = '3'
      &NfiCst.Title = 'CST'
   else
      &NfiCst.Title = 'CSOSN'
   endif

    If &NfsTipTrnNOP = 16  // Crédito
        BtnAlterarProduto.Visible = 1
    Else
        BtnAlterarProduto.Visible = 0
    EndIf

   &NfsTipoNfRef = 1 // NF-e
   Do 'ConfGridRef'

   &NfsEsp = 'UNIDADES'

   If &Logon.EmpClienteNa12 = 14 and &NfsTipTrnNOP = 15 and Null(&NfsQtd) // Anima Toys - Nota de Doação - Quantidade de volume zerada
       &NfsEsp = 'CAIXA'
       &NfsQtd = 1       
   EndIf
   If &Logon.EmpClienteNa12 = 6  //CANCIAN
       &NfsEsp = 'CAIXA'
       &NfsQtd =  &SdtNf.PedQtdeCaixa    
       &NfsPesoBruto = &SdtNf.PedQtdeKIlos
       //&NfsPesoLiquido = &SdtNf.PedQtdeKIlos
//   else
//       &NfsQtd    = &SdtNf.NfsQtd
   endif

    If &Logon.EmpClienteNa12 =11  //plasbrink
       &NfsEsp = 'CAIXA'          
    endif
   
endevent

event &NfsTpFrt.Click

if &NfsTpFrt <> 9
   &NfsTrpCod.Enabled = 1
   &NfsAntt.Enabled = 1
   &NfsPlcVei.Enabled = 1
   &NfsVeiUf.Enabled = 1
else
   &NfsTrpCod.Enabled = 0
   &NfsAntt.Enabled = 0
   &NfsPlcVei.Enabled = 0
   &NfsVeiUf.Enabled = 0
   &NfsForCpf = ''
   &NfsForCnpj = ''
   &NfsForEnd = ''
   &NfsForCidNom = ''
   &NfsForEstado = ''
   &NfsForIes = ''
   &NfsTrpCod = 0
   &NfsAntt = ''
   &NfsPlcVei = ''
   &NfsVeiUf = ''
endif

endevent

event &NfsTrpCod.Click

for each
  where TrpCod = &NfsTrpCod

  if TrpTp = 'J'
    &NfsForCnpj = TrpCnpj
    &NfsForCpf = ''
    &NfsForCnpj.Visible = 1
    &NfsForCpf.Visible = 0
  else
    &NfsForCpf = TrpCpf
    &NfsForCnpj = ''
    &NfsForCnpj.Visible = 0
    &NfsForCpf.Visible = 1
  endif

  &NfsForEnd = TrpEnd.Trim()+','+TrpEndNum.Trim()
  &NfsForCidNom = TrpCidNom
  &NfsForEstado = TrpUFCod
  &NfsForIes = TrpIe
  &NfsPlcVei = TrpPlaca
  &NfsAntt = TrpAntt
  &NfsVeiUf = TrpPlacaUF
  when none

  &NfsForCpf = ''
  &NfsForCnpj = ''
  &NfsForEnd = ''
  &NfsForCidNom = ''
  &NfsForEstado = ''
  &NfsForIes = ''
  &NfsTrpCod = 0
  &NfsPlcVei = ''
  &NfsAntt = ''
  &NfsVeiUf = ''

endfor

endevent


// Retirado dia 29/12/2017 pois a parte fiscal foi refeita, agora pode-se alterar a CFOP diretamente no pedido caso necessário.
//Event 'altpro'
//call(WNotaItem,&NfiPrdCod,&SdtNfPro,&NfsCliCod,&EmpUf,&NfsTpNf,&SdtNf)
//
//
//   &nfsvlricms = &SdtNf.NfsVlrIcms
//   &NfsVlrSt   = &SdtNf.NfsVlrSt
//   &NfsVlrDsc  = &SdtNf.NfsVlrDsc
//   &NfsBseClcIcms = &SdtNf.NfsBseClcIcms
//   &NfsBseClcSt = &SdtNf.NfsBseClcSt
//   &NfsVlrFrt = &SdtNf.NfsVlrFrt
//   &NfsVlrTotPrd = &SdtNf.NfsVlrTotPrd
//   &NfsVlrIpi = &SdtNf.NfsVlrIpi
//   &NfsVlrTotNf = &SdtNf.NfsVlrTotNf
//   &NfsVlrPis   = &SdtNf.nfsvlrpis
//   &NfsVlrCofins = &SdtNf.NfsVlrCofins
//   &NfsOutDsp = &SdtNf.NfsOutDsp
//   &NfsVlrDsc = &SdtNf.NfsVlrDsc
//   &NfsDesIcms = &SdtNf.NfsDesIcms
//
////
////&NfsBseClcIcms = 0
////&nfsvlricms = 0
////&NfsBseClcSt = 0
////&NfsVlrSt = 0
////&NfsVlrFrt = 0
////&NfsVlrTotPrd = 0
////&NfsVlrDsc = 0
////&NfsVlrPis = 0
////&NfsVlrCofins = 0
////&NfsVlrIpi = 0
////&NfsVlrTotNf = 0
////
////for &SdtNfProItem in &SdtNfPro
////
////    &NfsBseClcIcms += &SdtNfProItem.NfiBseClcIcms
////    &nfsvlricms += &SdtNfProItem.NfiVlrIcms
////    &NfsBseClcSt += &SdtNfProItem.NfiBseClcSt
////    &NfsVlrSt += &SdtNfProItem.NfiVlrSt
////    &NfsVlrFrt += &SdtNfProItem.NfiVlrFrete
////    &NfsVlrTotPrd += &SdtNfProItem.NfiTotPrd
////    &NfsVlrDsc += &SdtNfProItem.NfiVlrDsc
////    &NfsVlrPis += &SdtNfProItem.NfiVlrPis
////    &NfsVlrCofins += &SdtNfProItem.NfiVlrCof
////    &NfsVlrIpi += &SdtNfProItem.NfiVlrIpi
////    &NfsVlrTotNf += &SdtNfProItem.NfiVlrTot
////
////endfor
//   
//
//if &SdtNf.MovCcr = 1
//   do'acertaparc'
//endif
//
////&TotalParc = 0
////&NfpSeq = 0
////for &SdtNfParItem in &SdtNfPar
////
////   &NfpSeq += 1
////   &NfpVct = &SdtNfParItem.NfpVct
////   &NfpNumDoc = &SdtNfParItem.NfpNumDoc
////   &NfpVlr = &SdtNfParItem.NfpVlr
////
////   &TotalParc += &NfpVlr
////
////   GrdPar.Load()
////
////endfor
//
//&TotalParc = 0
//grdpar.Refresh()
//grdprd.Refresh()
////refresh
//
//
//EndEvent  // 'altpro'

Event 'vispro'
    WNotaItemDsp.call(&NfiSeq, &NfiPrdCod,&SdtNfPro,&SdtImp,&SdtADI, 'DSP')
EndEvent  // 'vispro'


Event 'AlterarProduto'
    call(WNotaItemDsp,&NfiSeq, &NfiPrdCod,&SdtNfPro,&SdtImp,&SdtADI,'UPD')

    &NfsBseClcIcms = 0
    &nfsvlricms = 0
    &NfsBseClcSt = 0
    &NfsVlrSt = 0
    &NfsVlrFrt = 0
    &NfsVlrTotPrd = 0
    &NfsVlrDsc = 0
    &NfsVlrPis = 0
    &NfsVlrCofins = 0
    &NfsVlrIpi = 0
    &NfsVlrTotNf = 0

    for &SdtNfProItem in &SdtNfPro

        &NfsBseClcIcms += &SdtNfProItem.NfiBseClcIcms
        &nfsvlricms += &SdtNfProItem.NfiVlrIcms
        &NfsBseClcSt += &SdtNfProItem.NfiBseClcSt
        &NfsVlrSt += &SdtNfProItem.NfiVlrSt
        &NfsVlrFrt += &SdtNfProItem.NfiVlrFrete
        &NfsVlrTotPrd += &SdtNfProItem.NfiTotPrd * &SdtNfProItem.NfiIndTot // // Quando o campo NfiIndTot for = 0 (Item não compõe o total da nota), não deve somar o valor do produto no total da nota e dos produtos
        &NfsVlrDsc += &SdtNfProItem.NfiVlrDsc
        &NfsVlrPis += &SdtNfProItem.NfiVlrPis
        &NfsVlrCofins += &SdtNfProItem.NfiVlrCof
        &NfsVlrIpi += &SdtNfProItem.NfiVlrIpi
        &NfsVlrTotNf += &SdtNfProItem.NfiVlrTot
        
    endfor
EndEvent  // 'AlterarProduto'

//event refresh
//&TotalParc = 0
//&NfpSeq = 0
//msg('refresh',status)
//endevent

event GrdPar.Load
    &TotalParc = 0   
    &NfpSeq = 0
    
    for &SdtNfParItem in &SdtNfPar
    
       &NfpSeq += 1
       &NfpVct = &SdtNfParItem.NfpVct
       &NfpNumDoc = &SdtNfParItem.NfpNumDoc
       &NfpVlr = &SdtNfParItem.NfpVlr
       &NfpVlr2 = &SdtNfParItem.NfpVlr2
       &obs     = &SdtNfParItem.OBS
       &formcod = &SdtNfParItem.FormCod
       &OpeSeq  = &SdtNfParItem.OpeSeq
       &OpePrc  = &SdtNfParItem.OpePrc
    
       &TotalParc += &NfpVlr
    
       Load
    endfor
endevent


event &OpeSeq.Click

if &FormCod = 10 or &FormCod = 11

    if &OpeSeq = 1 or &OpeSeq = 2

       msg('Operadora não pode ser VISA ou MASTER')
       &OpeSeq = 0

    else

        for each
            where OpeSeq = &OpeSeq
        
            if &FormCod = 10
               &OpePrc = OpePercCred
            endif
        
            if &FormCod = 11 
               &OpePrc = OpePercDeb
            endif
        
        endfor

    endif

endif


endevent

event &FormCod.Click

if &FormCod = 10 or &FormCod = 11
   &OpeSeq = 0
   &OpePrc = 0
endif

if &FormCod = 4 or &FormCod = 5

   &OpeSeq = 1
   
   for each
        where OpeSeq = 1

        if &FormCod = 5
           &OpePrc = OpePercCred
        endif
    
        if &FormCod = 4
           &OpePrc = OpePercDeb
        endif

   endfor

endif


if &FormCod = 8 or &FormCod = 9

   &OpeSeq = 2
   
   for each
        where OpeSeq = 2

        if &FormCod = 8
           &OpePrc = OpePercCred
        endif
    
        if &FormCod = 9
           &OpePrc = OpePercDeb
        endif

   endfor


endif

if &FormCod <> 4 and &FormCod <> 5 and &FormCod <> 8 and &FormCod <> 9 and &FormCod <> 10 and &FormCod <> 11 
   &OpePrc = 0
   &OpeSeq = 0
endif

endevent



event GrdPrd.Load

&SdtNfPro.Sort('NfiSeq')

for &SdtNfProItem in &SdtNfPro

    

    &NfiSeq = &SdtNfProItem.NfiSeq
    &NfiPrdCod = &SdtNfProItem.NfiPrdCod
    &NfiPrdDsc = &SdtNfProItem.NfiPrdDsc
    &NfiPrdNcmCod = &SdtNfProItem.NfiPrdNcmCod
    &NfiCst = &SdtNfProItem.NfiCst
    &NfiCfopCod = &SdtNfProItem.NfiCfopCod
    &NfiCfopSeq = &SdtNfProItem.NfiCfopSeq
    &NfiPrdUndCod = &SdtNfProItem.NfiPrdUndCod
    &NfiQtd = &SdtNfProItem.NfiQtd
    &NfiVlrUnt = &SdtNfProItem.NfiVlrUnt
    &NfiVlrTot = &SdtNfProItem.NfiVlrTot
    &NfiTotPrd = &SdtNfProItem.NfiTotPrd
    &NfiVlrIcms = &SdtNfProItem.NfiVlrIcms
    &NfiVlrIpi  = &SdtNfProItem.NfiVlrIpi
    &NfiAlqIcms = &SdtNfProItem.NfiAlqIcms
    &NfiAlqIpi  = &SdtNfProItem.NfiAlqIpi
    &NfiOrdCompra = &SdtNfProItem.NfiOrdCompra

    Load
endfor

//do'soma'


endevent

event grdref.Load
    &NfRefSeq = 0
    for &SdtNfChRefItem in &SdtNfChRef
        &NfRefSeq += 1
        &NfChRef2 = &SdtNfChRefItem.NfsCh      
        &NfRefCnpj = &SdtNfChRefItem.NfRefCnpj
        &NfRefCpf = &SdtNfChRefItem.NfRefCpf
        &NfRefIe = &SdtNfChRefItem.NfRefIe
        &NfRefMod = &SdtNfChRefItem.NfRefMod             
        &NfRefNum = &SdtNfChRefItem.NfRefNum
        &NfRefSerie = &SdtNfChRefItem.NfRefSerie
        &NfRefUf = &SdtNfChRefItem.NfRefUf
        &NfRefAnoMes = &SdtNfChRefItem.NfRefAnoMes
        &NfRefTipoDoc = &SdtNfChRefItem.NfRefTipoDoc
        &NfRefTipoNfRef = &SdtNfChRefItem.NfsTipoNfRef

        Do Case
            Case &NfRefTipoNfRef = 2 and &NfRefMod = '01' // NF modelo 1/1A
                &NfRefModDesc = 'modelo 01'
            Case &NfRefTipoNfRef = 2 and &NfRefMod = '02' // NF modelo 1/1A
                &NfRefModDesc = 'modelo 02'
            Case &NfRefTipoNfRef = 3 and &NfRefMod = '04' // NF de produtor rural
                &NfRefModDesc = 'NF de Produtor'
            Case &NfRefTipoNfRef = 3 and &NfRefMod = '01' // NF de produtor rural
                &NfRefModDesc = 'NF'
            OtherWise
                &NfRefModDesc = ''
        EndCase 

        grdref.Load()
    endfor
endevent




Event Enter

&TotalParc = 0
&erro = 0
for each line in grdpar

   &TotalParc += &NfpVlr

    if &FormCod = 4 or &FormCod = 5 or &FormCod = 8 or &FormCod = 9 or &FormCod = 10 or &FormCod = 11 
       if null(&OpeSeq)
          msg('Selecione a operadoda da parcela nº '+trim(str(&NfpSeq)))
          &erro = 1
       endif
    endif

endfor

PPrcCommit.Call()

// Verifica se a nota já existe (acontece quando fatura 2 pedidos ao mesmo tempo)
for each 
    where NfsNum = Val(&nota2)
    where NfsSer = &NfsSer   
        &erro = 2
endfor

Call(PVerStsPed, &Logon, &PedCod, &PedSts2)

If &PedSts2 <> 1
    &erro = 3
EndIf


do case
    case null(&NfsDtaEms)
       msg('Data de Emissão não informada')
    case null(&NfsDtaSai)
       msg('Data da Saída nao informada')
    case (null(&NfsCliCpf) or &NfsCliCpf = '   .   .   -  ') and &NfsCliTp = 'F'
       msg('CPF do Destinatário não informado')
    case (null(&NfsCliCNPJ) or &NfsCliCNPJ = '  .   .   /    -  ') and &NfsCliTp = 'J'
       msg('CNPJ do Destinatário não informado')
    case (&SdtNf.NfsTpTrn = 2 or &SdtNf.NfsTpTrn = 9) and (&SdtNfChRef.Count = 0)
       msg('Indique uma nota fiscal referênciada para prosseguir com a operação de devolução')
       &NfChRef.Setfocus()
    case &erro = 1
       msg('Erro Encontrado',status)
    case &erro = 2
       msg('Nota Fiscal Nº. '+ &nota2.trim()+' já existe no sistema !!!')
    case &erro = 3
       msg('Pedido Nº. '+ Trim(Str(&PedCod)) +' já está finalizado !!!')
    otherwise

    &SdtNf.NfsDtaEms = &NfsDtaEms
    &SdtNf.NfsDtaSai = &NfsDtaSai
    &SdtNf.NfsHraSai = &NfsHraSai

    &NfsBseClcIcms = 0
    &nfsvlricms = 0
    &NfsBseClcSt = 0
    &NfsVlrSt = 0
    &NfsVlrFrt = 0
    &NfsVlrTotPrd = 0
    &NfsVlrDsc = 0
    &NfsVlrPis = 0
    &NfsVlrCofins = 0
    &NfsVlrIpi = 0
    &NfsVlrTotNf = 0

    for &SdtNfProItem in &SdtNfPro

        &NfsBseClcIcms += &SdtNfProItem.NfiBseClcIcms
        &nfsvlricms += &SdtNfProItem.NfiVlrIcms
        &NfsBseClcSt += &SdtNfProItem.NfiBseClcSt
        &NfsVlrSt += &SdtNfProItem.NfiVlrSt
        &NfsVlrFrt += &SdtNfProItem.NfiVlrFrete
        &NfsVlrTotPrd += &SdtNfProItem.NfiTotPrd * &SdtNfProItem.NfiIndTot // // Quando o campo NfiIndTot for = 0 (Item não compõe o total da nota), não deve somar o valor do produto no total da nota e dos produtos
        &NfsVlrDsc += &SdtNfProItem.NfiVlrDsc
        &NfsVlrPis += &SdtNfProItem.NfiVlrPis
        &NfsVlrCofins += &SdtNfProItem.NfiVlrCof
        &NfsVlrIpi += &SdtNfProItem.NfiVlrIpi
        &NfsVlrTotNf += &SdtNfProItem.NfiVlrTot
        
    endfor

    &SdtNf.NfsBseClcIcms = &NfsBseClcIcms
    &SdtNf.NfsVlrIcms = &nfsvlricms 
    &SdtNf.NfsBseClcSt =  &NfsBseClcSt 
    &SdtNf.NfsVlrSt = &NfsVlrSt
    &SdtNf.NfsVlrFrt =  &NfsVlrFrt
    &SdtNf.NfsVlrTotPrd = &NfsVlrTotPrd
    &SdtNf.NfsVlrDsc = &NfsVlrDsc 
    &SdtNf.NfsVlrPis = &NfsVlrPis
    &SdtNf.NfsVlrCofins = &NfsVlrCofins
    &SdtNf.NfsVlrIpi = &NfsVlrIpi
    &SdtNf.NfsOutDsp = &NfsOutDsp
    &SdtNf.NfsVlrTotNf = &NfsVlrTotNf
    &SdtNf.NfsTrpCod = &NfsTrpCod
    &SdtNf.NfsCubagem = &NfsCubagem
 
    &SdtNf.NfsTpFrt = &NfsTpFrt
    &SdtNf.NfsAntt = &NfsAntt
    &SdtNf.NfsPlcVei = &NfsPlcVei
    &SdtNf.NfsVeiUf = &NfsVeiUf
    &SdtNf.NfsQtd = &NfsQtd
    &SdtNf.NfsEsp = &NfsEsp
    &SdtNf.NfsMarca = &NfsMarca
    &SdtNf.NfsNumeracao = &NfsNumeracao
    &SdtNf.NfsPesoBruto = &NfsPesoBruto
    &SdtNf.NfsPesoliquido = &NfsPesoLiquido
    &SdtNf.NfsInfCmp = &NfsInfCmp
    &SdtNf.NfsCfopSeq = &NfsCfopSeq
    &SdtNf.NfsNum = val(&nota2)
    &SdtNf.NfsSer = &NfsSer
    &SdtNf.NfsTpNf = &NfsTpNf
    &SdtNf.NfsUfEmb = &NfsUfEmb
    &SdtNf.NfsLocEmb = &NfsLocEmb
    &SdtNf.NfsDtaCont = &NfsDtaCont
    &SdtNf.NfsHraCont = &NfsHraCont
    &SdtNf.NfsJustCont = &NfsJustCont
    &SdtNf.NfsPla1Cod = &CpgPla1Cod
    &SdtNf.NfsPla2Cod = &CpgPla2Cod
    &SdtNf.NfsPla3Cod = &CpgPla3Cod
    &SdtNf.NfsPla4Cod = &CpgPla4Cod
    &SdtNf.NfsPla5Cod = &CpgPla5Cod
    &SdtNf.NfsDescCfop = &NfsDescCfop
    &SdtNf.NfsVlrIcmsDest = &NfsVlrIcmsDest
    &SdtNf.NfsVlrIcmsOri = &NfsVlrIcmsOri
    &SdtNf.NfsVlrFCP = &NfsVlrFCP
    &SdtNf.NfsNum = val(&nota2)
    &SdtNf.NfsVlrIPIDev = &NfsVlrIPIDev
    &SdtNf.NfsVlrFCPSub = &NfsVlrFCPSub
    &SdtNf.NfsTipoNfRef = &NfsTipoNfRef

    &SdtNfPar.Clear()
    for each line in grdpar
        &SdtNfParItem = new SdtNfPar.SdtNfParItem()
        &SdtNfParItem.NfpNumDoc = &NfpNumDoc
        &SdtNfParItem.NfpSeq    = &NfpSeq
        &SdtNfParItem.NfpVct    = &NfpVct
        &SdtNfParItem.NfpVlr    = &NfpVlr
        &SdtNfParItem.NfpVlr2   = &NfpVlr2
        &SdtNfParItem.FormCod   = &formcod
        &SdtNfParItem.OBS       = &obs
        &SdtNfParItem.OpePrc    = &OpePrc
        &SdtNfParItem.OpeSeq    = &OpeSeq
        &SdtNfPar.Add(&SdtNfParItem)
    endfor

     call(PGravaNota,&Logon,&SdtNf,&SdtNfPar,&SdtNfPro,&SdtNfChRef,&SdtImp,&SdtADI)
     &Gravou = 'S'

     return

endcase
EndEvent  // Enter

//sub'acertaparc'
//
//for &sdtnfparitem in &SdtNfPar
//
//    for each
//        where CondCod = &SdtNf.NfsCondCod
//
//        &CondNumPrc = CondNumPrc
//  
//        if &CondNumPrc >= 1 and &SdtNfParItem.NfpSeq = 1
//
//           &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc1 / 100),2)
//
//           if &SdtNf.NfsVlrSer > 0
//             &SdtNfParItem.NfpVlr2 = Round(&NfsVlrTotNf * (CondPorc1 / 100),2)
//           endif
//
//        endif
//
//        if &CondNumPrc >= 2 and &SdtNfParItem.NfpSeq = 2
//
//           &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc2 / 100),2)
//
//           if &SdtNf.NfsVlrSer > 0
//             &SdtNfParItem.NfpVlr2 = Round(&NfsVlrTotNf * (CondPorc2 / 100),2)
//           endif
//
//        endif
//        
//        if &CondNumPrc >= 3 and &SdtNfParItem.NfpSeq = 3
//
//           &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc3 / 100),2)
//
//           if &SdtNf.NfsVlrSer > 0
//             &SdtNfParItem.NfpVlr2 = Round(&NfsVlrTotNf * (CondPorc3 / 100),2)
//           endif
//
//        endif
//
//
//        if &CondNumPrc >= 4 and &SdtNfParItem.NfpSeq = 4
//
//           &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc4 / 100),2)
//
//           if &SdtNf.NfsVlrSer > 0
//             &SdtNfParItem.NfpVlr2 = Round(&NfsVlrTotNf * (CondPorc4 / 100),2)
//           endif
//
//        endif
//
//        if &CondNumPrc >= 5 and &SdtNfParItem.NfpSeq = 5
//
//           &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc5 / 100),2)
//
//           if &SdtNf.NfsVlrSer > 0
//             &SdtNfParItem.NfpVlr2 = Round(&NfsVlrTotNf * (CondPorc5 / 100),2)
//           endif
//
//        endif
//
//        if &CondNumPrc >= 6 and &SdtNfParItem.NfpSeq = 6
//
//           &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc6 / 100),2)
//
//           if &SdtNf.NfsVlrSer > 0
//             &SdtNfParItem.NfpVlr2 = Round(&NfsVlrTotNf * (CondPorc6 / 100),2)
//           endif
//
//        endif
//
//
//        if &CondNumPrc >= 7 and &SdtNfParItem.NfpSeq = 7
//
//           &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc7 / 100),2)
//
//           if &SdtNf.NfsVlrSer > 0
//             &SdtNfParItem.NfpVlr2 = Round(&NfsVlrTotNf * (CondPorc7 / 100),2)
//           endif
//
//        endif
//
//        if &CondNumPrc >= 8 and &SdtNfParItem.NfpSeq = 8
//
//           &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc8 / 100),2)
//
//           if &SdtNf.NfsVlrSer > 0
//             &SdtNfParItem.NfpVlr2 = Round(&NfsVlrTotNf * (CondPorc8 / 100),2)
//           endif
//
//        endif
//
//        if &CondNumPrc >= 9 and &SdtNfParItem.NfpSeq = 9
//
//           &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc9 / 100),2)
//
//           if &SdtNf.NfsVlrSer > 0
//             &SdtNfParItem.NfpVlr2 = Round(&NfsVlrTotNf * (CondPorc9 / 100),2)
//           endif
//
//        endif
//
//        if &CondNumPrc >= 10 and &SdtNfParItem.NfpSeq = 10
//
//           &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc10 / 100),2)
//
//           if &SdtNf.NfsVlrSer > 0
//             &SdtNfParItem.NfpVlr2 = Round(&NfsVlrTotNf * (CondPorc10 / 100),2)
//           endif
//
//        endif
//
//        if &CondNumPrc >= 11 and &SdtNfParItem.NfpSeq = 11
//
//           &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc11 / 100),2)
//
//           if &SdtNf.NfsVlrSer > 0
//             &SdtNfParItem.NfpVlr2 = Round(&NfsVlrTotNf * (CondPorc11 / 100),2)
//           endif
//
//        endif
//
//        if &CondNumPrc >= 12 and &SdtNfParItem.NfpSeq = 12
//
//           &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc12 / 100),2)
//
//           if &SdtNf.NfsVlrSer > 0
//             &SdtNfParItem.NfpVlr2 = Round(&NfsVlrTotNf * (CondPorc12 / 100),2)
//           endif
//
//        endif
//
//    endfor
//
//endfor
//
//&Total2 = 0
//for &SdtNfParItem in &SdtNfPar
//
//    &Total2 += round(&SdtNfParItem.NfpVlr,2)
//
//endfor
//
//
//if &Total2 <> &NfsVlrTotNf
//    
//       &dif = &NfsVlrTotNf - &Total2
//    
//       for &SdtNfParItem in &SdtNfPar
//    
//           &SdtNfParItem.NfpVlr += &dif
//           exit
//    
//       endfor
//    
//endif
//
//
//if &SdtNf.NfsVlrSer > 0
//
//    &Total2 = 0
//    for &SdtNfParItem in &SdtNfPar
//    
//        &Total2 += round(&SdtNfParItem.NfpVlr2,2)
//    
//    endfor
//    
//    
//    if &Total2 <> &SdtNf.NfsVlrSer
//        
//           &dif = &SdtNf.NfsVlrSer - &Total2
//        
//           for &SdtNfParItem in &SdtNfPar
//        
//               &SdtNfParItem.NfpVlr2 += &dif
//               exit
//        
//           endfor
//        
//    endif
//
//endif
//
//
//if &ConvDiaFec > 0
//
//   for &SdtNfParItem in &SdtNfPar
//
//       // se o dia de fechamento do convenio for maior que o dia de hoje não soma 1 no mes
//       if &ConvDiaFec > day(&SdtNfParItem.NfpVct)
//
//          &mes = Month(&SdtNfParItem.NfpVct)
//          &Ano = year(&SdtNfParItem.NfpVct)
//
//       else
//
//          if month(&SdtNfParItem.NfpVct) = 12 
//             &mes = Month(&SdtNfParItem.NfpVct) + 1
//             if &mes > 12
//                &mes = 1
//             endif
//             &Ano = year(&SdtNfParItem.NfpVct) + 1
//          else
//             &mes = month(&SdtNfParItem.NfpVct) + 1
//             &Ano = year(&SdtNfParItem.NfpVct)
//          endif
//
//       endif
//
//       &datac = padl(trim(str(&ConvDiaFec)),2,'0')+'/'+padl(trim(str(&mes)),2,'0')+'/'+trim(str(&Ano))
//       &SdtNfParItem.NfpVct = ctod(&datac)
//
//   endfor
//
//endif
//
//
//
////&SdtNfPar.Clear()
////&seq = 0
////for each
////   where CondCod = &SdtNf.NfsCondCod
////  
////   &CondNumPrc = CondNumPrc
////
////   if &CondNumPrc >= 1
////
////      &seq += 1
////      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
////      &SdtNfParItem.NfpSeq = &seq
//////      &SdtNfParItem.NfpVct = &Today + CondDia1
////      &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc1 / 100),2)
////
////      if &SdtNf.NfsVlrSer > 0
////         &SdtNfParItem.NfpVlr2 = Round(&NfsVlrTotNf * (CondPorc1 / 100),2)
////      endif
////
////      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
////      &SdtNfPar.Add(&SdtNfParItem)
////
////   endif
////
////   if &CondNumPrc >= 2
////
////       &seq += 1
////      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
////      &SdtNfParItem.NfpSeq = &seq
////      &SdtNfParItem.NfpVct = &Today + CondDia2
////      &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc2 / 100),2)      
////      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
////      &SdtNfPar.Add(&SdtNfParItem)
////
////   endif
////
////
////    if &CondNumPrc >= 3
////
////      &seq += 1
////      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
////      &SdtNfParItem.NfpSeq = &seq
////      &SdtNfParItem.NfpVct = &Today + CondDia3
////      &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc3 / 100),2)
////      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
////      &SdtNfPar.Add(&SdtNfParItem)
////
////
////   endif
////
////    if &CondNumPrc >= 4
////
////       &seq += 1
////      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
////      &SdtNfParItem.NfpSeq = &seq
////      &SdtNfParItem.NfpVct = &Today + CondDia4
////      &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc4 / 100),2)
////      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
////      &SdtNfPar.Add(&SdtNfParItem)
////
////   endif
////
////
////    if &CondNumPrc >= 5
////
////      &seq += 1
////      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
////      &SdtNfParItem.NfpSeq = &seq
////      &SdtNfParItem.NfpVct = &Today + CondDia5
////      &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc5 / 100),2)
////      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
////      &SdtNfPar.Add(&SdtNfParItem)
////
////
////   endif
////
////
////    if &CondNumPrc >= 6
////
////       &seq += 1
////      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
////      &SdtNfParItem.NfpSeq = &seq
////      &SdtNfParItem.NfpVct = &Today + CondDia6
////      &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc6 / 100),2)
////      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
////      &SdtNfPar.Add(&SdtNfParItem)
////
////   endif
////
////    if &CondNumPrc >= 7
////
////      &seq += 1
////      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
////      &SdtNfParItem.NfpSeq = &seq
////      &SdtNfParItem.NfpVct = &Today + CondDia7
////      &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc7 / 100),2)
////      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
////      &SdtNfPar.Add(&SdtNfParItem)
////
////
////   endif
////
////    if &CondNumPrc >= 8
////
////      &seq += 1
////      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
////      &SdtNfParItem.NfpSeq = &seq
////      &SdtNfParItem.NfpVct = &Today + CondDia8
////      &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc8 / 100),2)
////      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
////      &SdtNfPar.Add(&SdtNfParItem)
////
////
////   endif
////
////    if &CondNumPrc >= 9
////
////      &seq += 1
////      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
////      &SdtNfParItem.NfpSeq = &seq
////      &SdtNfParItem.NfpVct = &Today + CondDia9
////      &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc9 / 100),2)
////      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
////      &SdtNfPar.Add(&SdtNfParItem)
////
////
////   endif
////
////    if &CondNumPrc >= 10
////
////      &seq += 1
////      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
////      &SdtNfParItem.NfpSeq = &seq
////      &SdtNfParItem.NfpVct = &Today + CondDia10
////      &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc10 / 100),2)
////      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
////      &SdtNfPar.Add(&SdtNfParItem)
////
////   endif
////
////    if &CondNumPrc >= 11
////
////      &seq += 1
////      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
////      &SdtNfParItem.NfpSeq = &seq
////      &SdtNfParItem.NfpVct = &Today + CondDia11
////      &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc11 / 100),2)
////      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
////      &SdtNfPar.Add(&SdtNfParItem)
////
////
////   endif
////
////
////    if &CondNumPrc >= 12
////
////      &seq += 1
////      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
////      &SdtNfParItem.NfpSeq = &seq
////      &SdtNfParItem.NfpVct = &Today + CondDia12
////      &SdtNfParItem.NfpVlr = Round(&NfsVlrTotNf * (CondPorc12 / 100),2)
////      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
////      &SdtNfPar.Add(&SdtNfParItem)
////
////   endif
////   
////
////endfor
////
////&Total2 = 0
////for &SdtNfParItem in &SdtNfPar
////
////    &Total2 += round(&SdtNfParItem.NfpVlr,2)
////
////endfor
////
////
////if &Total2 <> &NfsVlrTotNf
////    
////       &dif = &NfsVlrTotNf - &Total2
////    
////       for &SdtNfParItem in &SdtNfPar
////    
////           &SdtNfParItem.NfpVlr += &dif
////           exit
////    
////       endfor
////    
////endif
//
//endsub

Event 'inc'
    Do Case
        Case &NfsTipoNfRef = 1 // NF-e

            if not null(&NfChRef)            
                &SdtNfChRefItem = new SdtNfChRef.SdtNfChRefItem()
                &SdtNfChRefItem.NfsCh = &NfChRef
                &SdtNfChRef.Add(&SdtNfChRefItem)
                
                &NfChRef = ''                            
            else
                Msg('Favor informar a chave de acesso referenciada!')
                &NfChRef.Setfocus()
            endif

        Case &NfsTipoNfRef = 2 // NF modelo 1/1A
            Call(WNotaRefMod1, &Logon, &SdtNfChRef, 0, '', 'INS')

        Case &NfsTipoNfRef = 3 // NF de produtor rural
            Call(WNotaRefProdRural, &Logon, &SdtNfChRef, 0, '', 'INS')

    EndCase

    grdref.Refresh()

EndEvent  // 'inc'

Event 'exc'
&SdtNfChRef.Remove(&NfRefSeq)
grdref.Refresh()
EndEvent  // 'exc'

Event 'sel'
call(WSelPla,&CpgPla1Cod,&CpgPla2Cod,&CpgPla3Cod,&CpgPla4Cod,&CpgPla5cOD)
for each
    where Pla1Cod = &CpgPla1Cod
    where Pla2Cod = &CpgPla2Cod
    where Pla3Cod = &CpgPla3Cod
    where Pla4Cod = &CpgPla4Cod
    Where Pla5Cod = &CpgPla5cOD

    &Pla5CodDesc = Pla5CodDesc
    &CpgPla4Dsc = Pla5Dsc

endfor
EndEvent  // 'sel'


//sub'soma'
//
//msg(&NfsBseClcIcms.ToString(),status)
//
//&NfsBseClcIcms.SetEmpty()
//&nfsvlricms.SetEmpty()
//&NfsBseClcSt.SetEmpty()
//&NfsVlrSt.SetEmpty()
//&NfsVlrFrt.SetEmpty()
//&NfsVlrTotPrd.SetEmpty()
//&NfsVlrDsc.SetEmpty()
//&NfsVlrPis.SetEmpty()
//&NfsVlrCofins.SetEmpty()
//&NfsVlrIpi.SetEmpty()
//&NfsVlrTotNf.SetEmpty()
//
////msg('Entrei')
//
//    for &SdtNfProItem in &SdtNfPro
//    
//        &NfsBseClcIcms += &SdtNfProItem.NfiBseClcIcms
//        &nfsvlricms += &SdtNfProItem.NfiVlrIcms
//        &NfsBseClcSt += &SdtNfProItem.NfiBseClcSt
//        &NfsVlrSt += &SdtNfProItem.NfiVlrSt
//        &NfsVlrFrt += &SdtNfProItem.NfiVlrFrete
//        &NfsVlrTotPrd += &SdtNfProItem.NfiTotPrd
//        &NfsVlrDsc += &SdtNfProItem.NfiVlrDsc
//        &NfsVlrPis += &SdtNfProItem.NfiVlrPis
//        &NfsVlrCofins += &SdtNfProItem.NfiVlrCof
//        &NfsVlrIpi += &SdtNfProItem.NfiVlrIpi
//        &NfsVlrTotNf += &SdtNfProItem.NfiVlrTot
//    
//    endfor
//endsub


sub'tipo'
for each
   where TpEndSeq = &EmpTpEndSeq

   &TpEndDsc = TpEndDsc.Trim()

endfor
endsub


Event &NfsTipoNfRef.Click
    Do 'ConfGridRef'
EndEvent


Sub 'ConfGridRef'
    Do Case
        Case &NfsTipoNfRef = 1 // NF-e

// Alteração 20/05/2019: Se alterar a visibilidade dos campos, o grid para de funcionar barra de rolagem vertical
//            &NfRefSeq.visible = 1
//            &NfChRef2.visible = 1       
//            &NfRefCnpj.visible = 0 
//            &NfRefCpf.visible = 0 
//            &NfRefIe.visible = 0 
//            &NfRefMod.visible = 0 
//            &NfRefModDesc.visible = 0 
//            &NfRefNum.visible = 0 
//            &NfRefSerie.visible = 0 
//            &NfRefUf.visible = 0
//            &NfRefAnoMes.Visible = 0
//            &NfRefTipoDoc.Visible = 0

            lblNfChRef.Visible = 1
            &NfChRef.visible = 1

        Case &NfsTipoNfRef = 2 // NF modelo 1/1A

//            &NfRefSeq.visible = 1
//            &NfChRef2.visible = 0 
//            &NfRefIe.visible = 0
//            &NfRefCnpj.visible = 1
//            &NfRefCpf.visible = 0 
//            &NfRefMod.visible = 0 
//            &NfRefModDesc.visible = 1 
//            &NfRefNum.visible = 1 
//            &NfRefSerie.visible = 1 
//            &NfRefUf.visible = 1
//            &NfRefAnoMes.Visible = 1
//            &NfRefTipoDoc.Visible = 0

            lblNfChRef.Visible = 0
            &NfChRef.visible = 0

        Case &NfsTipoNfRef = 3 // NF de produtor rural

//            &NfRefSeq.visible = 1
//            &NfChRef2.visible = 0
//            &NfRefIe.visible = 1
//            &NfRefCnpj.visible = 1
//            &NfRefCpf.visible = 1
//            &NfRefMod.visible = 0
//            &NfRefModDesc.visible = 1
//            &NfRefNum.visible = 1
//            &NfRefSerie.visible = 1
//            &NfRefUf.visible = 1
//            &NfRefAnoMes.Visible = 1
//            &NfRefTipoDoc.Visible = 1

            lblNfChRef.Visible = 0
            &NfChRef.visible = 0

    EndCase

//    grdref.Refresh()
EndSub

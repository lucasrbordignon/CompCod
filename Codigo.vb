event start

    Form.Caption = Form.Caption + ' - (' + Trim(&Pgmname) + ')'

   btndsp.TooltipText = 'Visualizar o Registro'

   &empcod = &Logon.EmpCod

   &NfsSer2 = &NfsSer

   &NfsCfopSeq.Clear()
   for each
      defined by CfopDsc

      &NfsCfopSeq.Additem(CfopSeq,CfopCod.Trim()+' - '+CfopDsc.Trim())

   endfor

   for each
       defined by TrpAtv

       &NfsTrpCod.Additem(TrpCod,TrpNom)

   endfor

   for each
      where NfsNum = &NfsNum
      where NfsSer = &NfsSer

      &PedCod = NfsNumPed
      &nfsvlricms = NfsVlrIcms
      &NfsVlrSt   = NfsVlrSt
      &NfsVlrDsc  = NfsVlrDsc
      &NfsBseClcIcms = NfsBseClcIcms
      &NfsBseClcSt = NfsBseClcSt
      &NfsVlrFrt = NfsVlrFrt
      &NfsVlrTotPrd = NfsVlrTotPrd
      &NfsVlrIpi = NfsVlrIpi
      &NfsVlrTotNf = NfsVlrTotNf
      &NfsVlrPis   = NfsVlrPis
      &NfsVlrCofins = NfsVlrCofins
      &NfsCliCod = NfsCliCod
      &NfsQtd    = NfsQtd
      &NfsInfCmp = NfsInfCmp
      &NfsPesoBruto = NfsPesoBruto
      &NfsPesoLiquido = NfsPesoLiquido
      &NfsUfEmb = NfsUfEmb
      &NfsPlcVei = NfsPlcVei
      &NfsVeiUf = NfsVeiUf
      &NfsDescCfop = NfsDescCfop
      &NfsOutDsp = NfsOutDsp
      &NfsAntt = NfsAntt
      &NfsTrpCod = NfsTrpCod
      &NfsCubagem = NfsCubagem

      &nota2 = NfsNum.ToString()
      &nota2 = padl(trim(&nota2),11,'0')

      &NfsTpNf = NfsTpNf
      &NfsDtaEms = NfsDtaEms
      &NfsDtaSai = NfsDtaSai
      &NfsHraSai = NfsHraSai

      &NfsCliCod = NfsCliCod
      &NfsVenCod = NfsVenCod

      &NfsTpFrt = NfsTpFrt
      &NfsCfopSeq = NfsCfopSeq
      &NfsForCod = NfsForCod
      &NfsInfCmp = NfsInfCmp
      &NfsUfEmb = NfsUfEmb
      &NfsDtaCont = NfsDtaCont
      &NfsHraCont = NfsHraCont
      &NfsJustCont = NfsJustCont
      &TotalParc = NfsTotPar
      &NfsQtd = NfsQtd
      &NfsEsp = NfsEsp
      &NfsMarca = NfsMarca
      &NfsNumeracao = NfsNumeracao
      &NfsVlrIcmsDest = NfsVlrIcmsDest
      &NfsVlrIcmsOri = NfsVlrIcmsOri
      &NfsVlrFCP = NfsVlrFCP
      &NfsDesIcms = NfsDesIcms
      &NfsVlrIPIDev = NfsVlrIPIDev
      &NfsVlrFCPSub = NfsVlrFCPSub
      &NfsTipoNfRef = NfsTipoNfRef

   endfor

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
   
   //Adição
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
     where CcrNumNf = &NfsNum
     &Pla5CodDesc = CcrPla5CodDesc
     &CpgPla4Dsc = CcrPla5Dsc
     exit

   endfor

   for each
      where EmpCod = &empcod

      &EmpNom = EmpNom
      &Bit  = loadbitmap(EmpLogoNome)
      &EmpTpEndSeq = EmpTpEndSeq
      do'Tipo'
      &End1  = &TpEndDsc + space(1) + trim(EmpEnd)+','+trim(EmpEndNum)+'-'+trim(EmpEndBai)
      &end2  = trim(EmpCidade)+','+trim(EmpUf)+'- CEP:'+TRIM(EmpCep)
      &End3 = 'Fone/Fax:'+trim(EmpFone)
      &EmpIe = EmpIE
      &EmpCnpj = EmpCnpj
      &EmpUf = EmpUf

      if EmpIEise = 'S'
         &EmpIe = 'ISENTO'
      endif
      
      If EmpCRT = '3'
          NfiCst.Title = 'CST'
      Else
          NfiCst.Title = 'CSOSN'
      EndIf

   endfor

   for each
      where UfCod = &EmpUf

      &UfInscSub = UfInscSub

   endfor
   
   for each
     where VenCod = &NfsVenCod

     &VenNom = VenNom

   endfor

   for each
      where CliCod = &NfsCliCod

      &NfsCliNom = CliNom

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

  endfor

  do'Transportadora'
endevent






Event 'vispro'
call(WNotaItem2,NfiSeq, NfiPrdCod,&NfsNum,&NfsSer,&SdtImp,&sdtAdi)
EndEvent  // 'vispro'




event grdref.Load
    &SeqNfRef = nullvalue(&SeqNfRef)
    Do 'LimpaVarGridRef'

//    If &NfsTipoNfRef = 1 // 1: NF-e
        for each
           where NfsNum = &NfsNum
           where NfsSer = &NfsSer
           defined by NfChRef
        
               &SeqNfRef += 1        
               &NfChRef2 = NfChRef        
               load        
        endfor

        Do 'LimpaVarGridRef'

//    Else // 2: NF modelo 1/1A ou 3: NF de produtor rural
        for each NfRefSeq
           where NfsNum = &NfsNum
           where NfsSer = &NfsSer
           defined by NfRefUf
        
                &NfRefSeq = NfRefSeq 
                &SeqNfRef += 1 
                &NfRefAnoMes = NfRefAnoMes 
                &NfRefCnpj = NfRefCnpj
                &NfRefCpf = NfRefCpf
                &NfRefIe = NfRefIe
                &NfRefMod = NfRefMod
                &NfRefNum = NfRefNum
                &NfRefSerie = NfRefSerie
                &NfRefUf = NfRefUf      
                &NfRefTipoDoc = NfRefTipoDoc
                &NfRefTipoNfRef = NfRefTipoNfRef
 
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

               load    
    
               Do 'LimpaVarGridRef'
        endfor
//    EndIf
endevent





sub'Transportadora'
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
      when none
    
      &NfsForCpf = ''
      &NfsForCnpj = ''
      &NfsForEnd = ''
      &NfsForCidNom = ''
      &NfsForEstado = ''
      &NfsForIes = ''
      &NfsTrpCod = 0
    
    endfor
endsub


sub'tipo'
    for each
       where TpEndSeq = &EmpTpEndSeq
    
       &TpEndDsc = TpEndDsc.Trim()
    
    endfor
endsub


 Sub 'LimpaVarGridRef'        
    &NfRefSeq = nullvalue(&NfRefSeq)     
    &NfRefAnoMes = nullvalue(&NfRefAnoMes)
    &NfRefCnpj = nullvalue(&NfRefCnpj)
    &NfRefCpf = nullvalue(&NfRefCpf)
    &NfRefIe = nullvalue(&NfRefIe)
    &NfRefMod = nullvalue(&NfRefMod)
    &NfRefNum = nullvalue(&NfRefNum)
    &NfRefSerie = nullvalue(&NfRefSerie)
    &NfRefUf = nullvalue(&NfRefUf)
    &NfRefTipoDoc = nullvalue(&NfRefTipoDoc)
    &NfRefTipoNfRef = nullvalue(&NfRefTipoNfRef)
    &NfChRef2 = nullvalue(&NfChRef2)
  EndSub

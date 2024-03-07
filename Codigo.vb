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
           where TrpAtv = 1
           defined by TrpAtv
                &NfsTrpCod.Additem(TrpCod,TrpNom)
       endfor
       
    
       for each
          where NfsNum = &NfsNum
          where NfsSer = &NfsSer
           
          &PedCod = NfsNumPed
          &nfsvlricms = NfsVlrIcms
          &NfsVlrSt   = NfsVlrSt
          &NfsDescCfop = NfsDescCfop
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
          &NfsOutDsp = NfsOutDsp
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
          &NfsAntt = NfsAntt
          &NfsVlrIcmsDest = NfsVlrIcmsDest
          &NfsVlrIcmsOri = NfsVlrIcmsOri
          &NfsVlrFCP = NfsVlrFCP
          &NfsDesIcms = NfsDesIcms
          &NfsVlrIPIDev = NfsVlrIPIDev
          &NfsVlrFCPSub = NfsVlrFCPSub
          &NfsTipoNfRef = NfsTipoNfRef
    
          if &NfsTpFrt <> 9
               &NfsTrpCod.Enabled = 1
               &NfsAntt.Enabled = 1
               &NfsPlcVei.Enabled = 1
               &NfsVeiUf.Enabled = 1
               &NfsVeiUf = NfsVeiUf
               &NfsPlcVei = NfsPlcVei
               &NfsAntt = NfsAntt
               Do 'DadosTransportadoraIni'
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
          do'tipo'
          &End1  = &TpEndDsc+space(1)+trim(EmpEnd)+','+trim(EmpEndNum)+'-'+trim(EmpEndBai)
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

      Do 'ConfGridRef'
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
  do'DadosTransportadoraAlt'
endevent


Event 'vispro'
    call(WNotaItem2,NfiSeq,NfiPrdCod,&NfsNum,&NfsSer)
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


Event Enter




do case
    case null(&NfsDtaEms)
       msg('Data de Emissão não informada')
    case null(&NfsDtaSai)
       msg('Data da Saída nao informada')
    otherwise

    &SdtNf.NfsDtaEms = &NfsDtaEms
    &SdtNf.NfsDtaSai = &NfsDtaSai
    &SdtNf.NfsHraSai = &NfsHraSai

    &SdtNf.NfsForCod = &NfsForCod
    &SdtNf.NfsTpFrt = &NfsTpFrt
    &SdtNf.NfsAntt = &NfsAntt
    &SdtNf.NfsPlcVei = &NfsPlcVei
    &SdtNf.NfsQtd = &NfsQtd
    &SdtNf.NfsEsp = &NfsEsp
    &SdtNf.NfsMarca = &NfsMarca
    &SdtNf.NfsNumeracao = &NfsNumeracao
    &SdtNf.NfsPesoBruto = &NfsPesoBruto
    &SdtNf.NfsPesoliquido = &NfsPesoLiquido
    &SdtNf.NfsInfCmp = &NfsInfCmp
    &SdtNf.NfsUfEmb = &NfsUfEmb
    &SdtNf.NfsLocEmb = &NfsLocEmb
    &SdtNf.NfsDtaCont = &NfsDtaCont
    &SdtNf.NfsHraCont = &NfsHraCont
    &SdtNf.NfsJustCont = &NfsJustCont
    &SdtNf.NfsDescCfop = &NfsDescCfop
    &SdtNf.NfsTrpCod = &NfsTrpCod
    &SdtNf.NfsCubagem = &NfsCubagem
    &SdtNf.NfsVeiUf = &NfsVeiUf
    &SdtNf.NfsTipoNfRef = &NfsTipoNfRef

     call(PGravaNota2,&Logon,&SdtNf,&NfsNum,&NfsSer)

     return

endcase
EndEvent  // Enter


Event 'inc'
    Do Case
        Case &NfsTipoNfRef = 1 // NF-e

            if not null(&nfchref)    
                for each
                    where NfsNum = &NfsNum
                    where NfsSer = &NfsSer
                    where NfChRef = &NfChRef        
                        msg('Chave de Acesso Digitada ja foi cadastrada !!!')        
                when none        
                    &NfChRef = strreplace(&NfChRef,'-','')
                    &NfChRef = strreplace(&NfChRef,'.','')
                    &NfChRef = strreplace(&NfChRef,' ','')
                    Call(PGravaChaveRef, &NfsNum,&NfsSer,&nfchref)            
                endfor
        
                &nfchref.SetEmpty()    
            endif

        Case &NfsTipoNfRef = 2 // NF modelo 1/1A
            Call(WNotaRefMod1, &Logon, &SdtNfChRef, &NfsNum, &NfsSer, 'UPD')

        Case &NfsTipoNfRef = 3 // NF de produtor rural
            Call(WNotaRefProdRural, &Logon, &SdtNfChRef, &NfsNum, &NfsSer, 'UPD')

    EndCase
        
    grdref.Refresh()
EndEvent  // 'inc'


Event 'exc'
    If &NfRefTipoNfRef = 2 or &NfRefTipoNfRef = 3
        Call(PNotaRefMod1_exclui, &Logon, &NfsNum, &NfsSer, &NfRefSeq) 
        grdref.Refresh()
    Else
        Call(PExcluiChaveRef, &NfsNum,&NfsSer,&NfChRef2)
        grdref.Refresh()
    EndIf
EndEvent  // 'exc'


sub'DadosTransportadoraAlt'
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
          &NfsAntt = TrpAntt
          &NfsVeiUf = TrpPlacaUF
          &NfsPlcVei = TrpPlaca
      when none    
          &NfsForCpf = ''
          &NfsForCnpj = ''
          &NfsForEnd = ''
          &NfsForCidNom = ''
          &NfsForEstado = ''
          &NfsForIes = ''
          &NfsTrpCod = 0
          &NfsVeiUf = ''
          &NfsAntt = ''
          &NfsPlcVei = ''    
    endfor
endsub


Sub'DadosTransportadoraIni'
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
EndSub





sub'tipo'
for each
   where TpEndSeq = &EmpTpEndSeq

   &TpEndDsc = trim(TpEndDsc)

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


Event 'AlterarProduto'

EndEvent  // 'AlterarProduto'

 
for each 
   where NfsNum = &SdtNf.NfsNum
   where NfsSer = &SdtNf.NfsSer

   msg('Nota Fiscal No.'+ &SdtNf.NfsNum.ToString()+' já existe no sistema !!!')
   return
endfor

for each
   where EmpCod = &Logon.EmpCod

       if &SdtNf.NfsNum > EmpUltNfs
          EmpUltNfs = &SdtNf.NfsNum
       endif
    
       &EmpTipoLote = EmpTipoLote
       &EmpPla1CodCcr = EmpPla1CodCcr
       &EmpPla2CodCcr = EmpPla2CodCcr
       &EmpPla3CodCcr = EmpPla3CodCcr
       &EmpPla4CodCcr = EmpPla4CodCcr
       &EmpPla5CodCcr = EmpPla5CodCcr
       &EmpCreCodContabil = EmpCreCodContabil
       &EmpNaoBaixaTitCcrCartao = EmpNaoBaixaTitCcrCartao
       &EmpTpMovStqCcr = EmpTpMovStqCcr

endfor

for each
   where PedCod = &SdtNf.NfsNumPed

   &PedCondTp = PedCondTp
   &PedMovCcr = PedMovCcr
   &PedMovStq = PedMovStq
   &PedGrpDespCod = PedGrpDespCod
   &PedFormCod = PedFormCod
   &PedFormPgt = PedFormPgt
   &pednumecf = PedNumECF
   &PedOpeCartao = PedOpeCartao
   &PedCliNom = PedCliNom
   &UsrCxCod = PedCaixa
   &PedDta = PedDta
   &PedTipTrnEstoqueTerceiro = PedTipTrnEstoqueTerceiro
   &PedCliCnpj = PedCliCnpj
   &PedCliCpf = PedCliCpf
   &PedCliTp = PedCliTp

   If not null(PedCreCod) // Se tiver credibilidade, aplica a porcentagem no valor da base da comissão
       &PedCreCod = PedCreCod
       Do 'Credibilidade'
       &PedVlrBaseCms = Round( PedVlrBaseCms * &CreVlr / 100 ,2)
   Else
       &PedVlrBaseCms = PedVlrBaseCms
   EndIf

endfor

if null(&UsrCxCod)

    for each
       where UsrCod = &Logon.UsrCod
    
       &UsrCxCod = UsrCxCod
    
    endfor

endif

if null(&UsrCxCod)
   &UsrCxCod = '99'
endif

for each
   where CfopSeq = &SdtNf.NfsCfopSeq

   &CfopDsc = &CfopDsc

endfor





new
    NfsNum = &SdtNf.NfsNum
    NfsSer = &SdtNf.NfsSer
    NfsEmpCod = &SdtNf.NfsEmpCod
    NfsCliCod = &SdtNf.NfsCliCod
    NfsDescCfop = &SdtNf.NfsDescCfop

    if not null(&SdtNf.NfsForCod)
       NfsForCod = &SdtNf.NfsForCod
    else
       NfsForCod.SetNull()
    endif

    if not null(&SdtNf.NfsVenCod)
        NfsVenCod = &SdtNf.NfsVenCod
    else
        NfsVenCod.SetNull()
    endif

    NfsCfopSeq = &SdtNf.NfsCfopSeq
    NfsTpNf = &SdtNf.NfsTpNf
    NfsSts = 'N'
    NfsTpFrt = &SdtNf.NfsTpFrt
    NfsAntt = &SdtNf.NfsAntt
    NfsPlcVei = &SdtNf.NfsPlcVei
    NfsVeiUf = &SdtNf.NfsVeiUf
    NfsQtd = &SdtNf.NfsQtd
    NfsEsp = &SdtNf.NfsEsp
    NfsMarca = &SdtNf.NfsMarca
    NfsNumeracao = &SdtNf.NfsNumeracao
    NfsPesoBruto = &SdtNf.NfsPesoBruto
    NfsPesoLiquido = &SdtNf.NfsPesoLiquido
    NfsDtaEms = &SdtNf.NfsDtaEms
    NfsDtaSai = &SdtNf.NfsDtaSai
    NfsHraSai = &SdtNf.NfsHraSai
    NfsBseClcIcms = &SdtNf.NfsBseClcIcms
    NfsVlrIcms = &SdtNf.NfsVlrIcms
    NfsBseClcSt = &SdtNf.NfsBseClcSt
    NfsVlrSt = &SdtNf.NfsVlrSt
    NfsVlrFrt = &SdtNf.NfsVlrFrt
    NfsVlrTotPrd = &SdtNf.NfsVlrTotPrd
    NfsVlrDsc = &SdtNf.NfsVlrDsc
    NfsNumPed = &SdtNf.NfsNumPed
    NfsBseClcIpi = &SdtNf.NfsBseClcIpi
    NfsVlrIpi = &SdtNf.NfsVlrIpi
    NfsVlrTotNf = &SdtNf.NfsVlrTotNf
    NfsTotSer = &SdtNf.NfsTotSer
    NfsBseClcISSQN = &SdtNf.NfsBseClcISSQN
    NfsVlrISSQN = &SdtNf.NfsVlrISSQN
    NfsInfCmp = substr(&SdtNf.NfsInfCmp,1,1500)
    NfsOutDsp = &SdtNf.NfsOutDsp
    NfsAcrescimos = &SdtNf.NfsAcrescimos
    NfsCredibilidade = &SdtNf.NfsCredibilidade
    NfsCubagem = &SdtNf.NfsCubagem
    NfsTrpCod = &SdtNf.NfsTrpCod

    if not null(&SdtNf.NfsUfEmb)
       NfsUfEmb = &SdtNf.NfsUfEmb
    else
       NfsUfEmb.SetNull()
    endif

    NfsLocEmb = &SdtNf.NfsLocEmb
    NfsJustCont = &SdtNf.NfsJustCont
    NfsHraCont = &SdtNf.NfsHraCont
    NfsDtaCont = &SdtNf.NfsDtaCont
    NfsDesIcms = &SdtNf.NfsDesIcms
    NfsDesPis = &SdtNf.NfsDesPis
    NfsDesCofins = &SdtNf.NfsDesCofins
    NfsTotIcmsDes = &SdtNf.NfsTotIcmsDes
    NfsTotCredIcms = &SdtNf.NfsTotCredIcms
    NfsVlrPis = &SdtNf.NfsVlrPis
    NfsVlrCofins = &SdtNf.NfsVlrCofins
    NfsTpTrn = &SdtNf.NfsTpTrn
    NfsTotTrib = &SdtNf.NfsTotTrib
    NfsOpeCartao = &SdtNf.NfsOpeCartao
    NfsOpePrc = &SdtNf.NfsOpePrc
    NfsNumEcf = &pednumecf
    NfsTotTribEst = &SdtNf.NfsTotTribEst
    NfsTotTribMun = &SdtNf.NfsTotTribMun
    NfsTotTribFedImp = &SdtNf.NfsTotTribFedImp
    NfsTotTribFedNac = &SdtNf.NfsTotTribFedNac
    NfsVlrIcmsDest = &SdtNf.NfsVlrIcmsDest
    NfsVlrIcmsOri = &SdtNf.NfsVlrIcmsOri
    NfsVlrFCP = &SdtNf.NfsVlrFCP
    NfsConsFinal = &SdtNf.NfsConsFinal
    NfsVlrFCPSub = &SdtNf.NfsVlrFCPSub
    NfsVlrIPIDev = &SdtNf.NfsVlrIPIDev
    NfsTipoNfRef = &SdtNf.NfsTipoNfRef
    NfsVlrII = &SdtNf.NfsVlrII
    NfsBseClcIcms = &SdtNf.NfsBseClcIcms
endnew


for &SdtNfChRefItem in &SdtNfChRef

    If &SdtNfChRefItem.NfsTipoNfRef = 2 or &SdtNfChRefItem.NfsTipoNfRef = 3 // 2: NF modelo 1/1A ou 3: NF de produtor rural
        Do 'UltNfRefSeq'
        new
            NfsNum = &SdtNf.NfsNum
            NfsSer = &SdtNf.NfsSer
            NfRefSeq = &NfRefSeq
            NfRefAnoMes = &SdtNfChRefItem.NfRefAnoMes 
            NfRefCnpj = &SdtNfChRefItem.NfRefCnpj
            NfRefCpf = &SdtNfChRefItem.NfRefCpf
            NfRefIe = &SdtNfChRefItem.NfRefIe
            NfRefMod = &SdtNfChRefItem.NfRefMod
            NfRefNum = &SdtNfChRefItem.NfRefNum
            NfRefSerie = &SdtNfChRefItem.NfRefSerie 
            NfRefUf = &SdtNfChRefItem.NfRefUf
            NfRefTipoDoc = &SdtNfChRefItem.NfRefTipoDoc
            NfRefTipoNfRef = &SdtNfChRefItem.NfsTipoNfRef
        endnew
    Else // 1: Nf-e
        new
            NfsNum = &SdtNf.NfsNum
            NfsSer = &SdtNf.NfsSer
            NfChRef = &SdtNfChRefItem.NfsCh
        when duplicate
            Msg('Chave de Acesso da NF Referenciada duplicada!')
        endnew
    EndIf

endfor

&cont = 0
for &SdtNfParItem in &SdtNfPar

&cont += 1
new
    NfsNum = &SdtNf.NfsNum
    NfsSer = &SdtNf.NfsSer
    NfpSeq = &cont
    NfpNumDoc = &SdtNfParItem.NfpNumDoc
    NfpVct = &SdtNfParItem.NfpVct
    NfpVlr = &SdtNfParItem.NfpVlr
    NfpFormCod = &SdtNfParItem.FormCod
    NfpOpeSeq = &SdtNfParItem.OpeSeq
    NfpOpePrc = &SdtNfParItem.OpePrc
endnew

endfor



&SdtNfPro.Sort('NfiSeq')

for &SdtNfProItem in &SdtNfPro

    new
        NfsNum = &SdtNf.NfsNum
        NfsSer = &SdtNf.NfsSer
        NfiSeq = &SdtNfProItem.NfiSeq
        NfiPrdCod = &SdtNfProItem.NfiPrdCod
        NfiCst = &SdtNfProItem.NfiCst
        NfiCfopSeq = &SdtNfProItem.NfiCfopSeq
        NfiQtd = &SdtNfProItem.NfiQtd
        NfiVlrUnt = &SdtNfProItem.NfiVlrUnt
        NfiVlrTot = &SdtNfProItem.NfiVlrTot
        NfiTotPrd = &SdtNfProItem.NfiTotPrd
        NfiBseClcIcms = &SdtNfProItem.NfiBseClcIcms
        NfiVlrIcms = &SdtNfProItem.NfiVlrIcms
        NfiAlqIcms = &SdtNfProItem.NfiAlqIcms
        NfiBseClcSt = &SdtNfProItem.NfiBseClcSt
        NfiVlrSt = &SdtNfProItem.NfiVlrSt
        NfiVlrFrete = &SdtNfProItem.NfiVlrFrete
        NfiCstIpi = &SdtNfProItem.NfiCstIpi
        NfiVlrIpi = &SdtNfProItem.NfiVlrIpi
        NfiAlqIpi = &SdtNfProItem.NfiAlqIpi
        NfiObs = &SdtNfProItem.NfiObs
        NfiIndTot = &SdtNfProItem.NfiIndTot
        NfiIcmsDes = &SdtNfProItem.NfiIcmsDes
        NfiDesIcms = &SdtNfProItem.NfiDesIcms
        NfiDesPis = &SdtNfProItem.NfiDesPis
        NfiDesCofins = &SdtNfProItem.NfiDesCofins
        NfiVlrCredIcms = &SdtNfProItem.NfiVlrCredIcms
        NfiCstPis = &SdtNfProItem.NfiCstPis
        NfiCstCof = &SdtNfProItem.NfiCstCof
        NfiAlqPis = &SdtNfProItem.NfiAlqPis
        NfiAlqCof = &SdtNfProItem.NfiAlqCof
        NfiVlrPis = &SdtNfProItem.NfiVlrPis
        NfiVlrCof = &SdtNfProItem.NfiVlrCof
        NfiVlrDsc = &SdtNfProItem.NfiVlrDsc
        NfiObs = &SdtNfProItem.NfiObs        
        NfiTotTrib = &SdtNfProItem.NfiTotTrib
        NfiOutDsp = &SdtNfProItem.NfiOutDesp
        NfiTotTribEst = &SdtNfProItem.NfiTotTribEst
        NfiTotTribMun = &SdtNfProItem.NfiTotTribMun
        NfiTotTribFedImp = &SdtNfProItem.NfiTotTribFedImp
        NfiTotTribFedNac = &SdtNfProItem.NfiTotTribFedNac
        NfiAcrescimos = &SdtNfProItem.NfiAcrescimos
        NfiFCI = &SdtNfProItem.NfiFcI
        NfiPerDev = &SdtNfProItem.NfiPerDev
        NfiVlrIpiDev = &SdtNfProItem.NfiVlrIpiDev
        NfiPerDiferimento = &SdtNfProItem.NfiPerDiferimento
        NfiVlrDiferimento = &SdtNfProItem.NfiVlrDiferimento
        NfiPerRedBc = &SdtNfProItem.NfiPerRedBc
        NfiBseClcIcmsInt = &SdtNfProItem.NfiBseClcIcmsInt
        NfiVlrIcmsDiferimento = &SdtNfProItem.NfiVlrIcmsDiferimento
        NfiVlrBseIcmsDest = &SdtNfProItem.NfiVlrBseIcmsDest
        NfiPerFCP = &SdtNfProItem.NfiPerFCP
        NfiVlrFCP = &SdtNfProItem.NfiVlrFCP
        NfiAlqInter = &SdtNfProItem.NfiAlqInter
        NfiVlrIcmsDest = &SdtNfProItem.NfiVlrIcmsDest
        NfiVlrIcmsOri = &SdtNfProItem.NfiVlrIcmsOri
        NfiAlqSt = &SdtNfProItem.NfiAlqSt
        NfiBseClcCOFINS = &SdtNfProItem.NfiBseClcCOFINS
        NfiBseClcIPI = &SdtNfProItem.NfiBseClcIPI
        NfiBseClcPIS = &SdtNfProItem.NfiBseClcPIS
        NfiAlqInterestadual = &SdtNfProItem.NfiAlqInterestadual
        NfiVlrBseClcFCPSub = &SdtNfProItem.NfiVlrBseClcFCPSub
        NfiVlrFCPSub = &SdtNfProItem.NfiVlrFCPSub
        NfiPercFCPSub = &SdtNfProItem.NfiPercFCPSub
        NfiBseClcSt060 = &SdtNfProItem.NfiBseClcSt060
        NfiVlrSt060 = &SdtNfProItem.NfiVlrSt060
        NfiAliqSt060 = &SdtNfProItem.NfiAliqSt060
        NfiVlrBseClcFcpStRet = &SdtNfProItem.NfiVlrBseClcFcpStRet
        NfiPercFcpStRet = &SdtNfProItem.NfiPercFcpStRet
        NfiVlrFcpStRet = &SdtNfProItem.NfiVlrFcpStRet
        NfiVlrBseClcIcmsEfet = &SdtNfProItem.NfiVlrBseClcIcmsEfet
        NfiPercIcmsEfet = &SdtNfProItem.NfiPercIcmsEfet
        NfiVlrIcmsEfet = &SdtNfProItem.NfiVlrIcmsEfet
        NfiVlrIcmsSubstituto = &SdtNfProItem.NfiVlrIcmsSubstituto
        NfiParcelaIcmsRetido = &SdtNfProItem.NfiParcelaIcmsRetido 
        NfiOrdCompra = &SdtNfProItem.NfiOrdCompra
        NfiIVlrBCImp = &SdtNfProItem.NfiIVlrBCImp
        NfiVlrDespAdu = &SdtNfProItem.NfiVlrDespAdu
        NfiVlrIOF = &SdtNfProItem.NfiVlrIOF
        NfiVlrII = &SdtNfProItem.NfiVlrII
    endnew

    &Cont2 = 0
    for &SdtImpItem in &SdtImp
        if &SdtImpItem.NfiPrdCod = &SdtNfProItem.NfiPrdCod
            new
                NfsNum = &SdtNf.NfsNum
                NfsSer = &SdtNf.NfsSer
                NfiSeq = &SdtImpItem.NfiSeq
                NfiImpSeq = &SdtImpItem.NfiImpSeq
                NfiImpNroDI = &SdtImpItem.NfiImpNroDI
                NfiImpDtaDI = &SdtImpItem.NfiImpDtaDI
                NfiImpLocDesemb = &SdtImpItem.NfiImpLocDesemb
                NfiImpUFDesemb = &SdtImpItem.NfiImpUFDesemb
                NfiImpDtaDesemb = &SdtImpItem.NfiImpDtaDesemb
                NfiImpcExportador = &SdtImpItem.NfiImpcExportador
                NfiImpViaTransp = &SdtImpItem.NfiImpViaTransp
                NfiImpVlrViaTransp = &SdtImpItem.NfiImpVlrViaTransp
                NfiImpTpIntermedio = &SdtImpItem.NfiImpTpIntermedio
                NfiImpCNPJ = &SdtImpItem.NfiImpCNPJ
                NfiImpUFTer = &SdtImpItem.NfiImpUFTer
                NfiImpVlrAFRMM = &SdtImpItem.NfiImpVlrAFRMM 
            endnew

            for &SdtADIItem in &SdtADI
              if &SdtADIItem.NfiSeq = &SdtImpItem.NfiSeq and &SdtADIItem.NfiImpSeq = &SdtImpItem.NfiImpSeq  
                new
                  NfiSeq           = &SdtADIItem.NfiSeq
                  NfiImpAdcSeq     = &SdtADIItem.NfiImpAdcSeq 
                  NfiImpSeq        = &SdtADIItem.NfiImpSeq
                  NfiImpAdcFab     = &SdtADIItem.NfiImpAdcFab
                  NfiImpAdcNro     = &SdtADIItem.NfiImpAdcNro
                  NfiImpAdcNroDraw = &SdtADIItem.NfiImpAdcNroDraw
                  NfiImpAdcVlrDesc = &SdtADIItem.NfiImpAdcVlrDesc
                endnew
      
              endif
            endfor

        endif
    endfor

    If &PedTipTrnEstoqueTerceiro = 1 // Movimenta estoque de terceiro
        Do 'BuscaTerceiro'

        &SdtEstoqueTerceiro.MovTerDta = &Today
        &SdtEstoqueTerceiro.MovTerTerCod = &TerCod
        &SdtEstoqueTerceiro.MovTerNfEntrada = &SdtNfProItem.PdiNfEntrada
        &SdtEstoqueTerceiro.MovTerNfSaida = &SdtNf.NfsNum
        &SdtEstoqueTerceiro.MovTerTp = 'S'
        &SdtEstoqueTerceiro.MovTerAutMan = 'A'
        &SdtEstoqueTerceiro.MovTerObs = 'Saída Automática: ' + &SdtNf.NfsDescCfop
        &SdtEstoqueTerceiro.MovTerQtd = &SdtNfProItem.NfiQtd
        &SdtEstoqueTerceiro.MovTerVlrUnt = &SdtNfProItem.NfiVlrUnt
        &SdtEstoqueTerceiro.MovTerPrdCod = &SdtNfProItem.NfiPrdCod    
   
        Call(PGravaEstoqueTerceiro, &Logon, &SdtEstoqueTerceiro, &MovTerSeq)
    Else 
        if &PedMovStq = 1 or &SdtNfProItem.PdiCfopMovStq = 'S' // Movimenta estoque principal
    
            &Flag = 'N'
            // Se for nota fiscal de cupom
            if &pednumecf <> 0
               do'locest'
            endif
    
            // se não existir lançamentos com esse pedido
            if &Flag = 'N'            
               
                if &SdtNf.NfsTpNf = 0 
                   &MovTp = 'E'
                endif
        
                if &SdtNf.NfsTpNf = 1
                   &MovTp = 'S'
                endif
    
                &PrdCod = &SdtNfProItem.NfiPrdCod
    
                Do 'VerificaProdBxStq' // Verifica no cadastro de produto se será baixado outro produto no lugar do produto principal
    
                Do 'VerificaBaixaComp' // Verifica no cadastro de composição se será baixado a composição ao invés do produto principal
    
                If &PrdBaixaCompVenda = 'N'
    
                    &PedCod2 = udp(PSequenciais,'Estoque')
        
                    // Saida no Estoque
                    new 
                       MovSeq = &PedCod2
                       MovPrdCod = &MovPrdCod
                       MovQtd = &SdtNfProItem.NfiQtd
                       MovDta = &SdtNf.NfsDtaEms
                       MovTp = &MovTp
                       MovVlr = &SdtNfProItem.NfiVlrUnt
                       MovDocNum = &SdtNf.NfsNumPed
                       MovUsr = &Logon.UsrCod
                       MovTms = now()
                       MovNfsNum = &SdtNf.NfsNum
                       MovNfsSer = &SdtNf.NfsSer
                       MovObs = &SdtNf.NfsDescCfop
                       MovIdReg = 'V'
                       MovEcfNum = &pednumecf
        
                       &LOGHST = 'MOVIMENTO DE ESTOQUE: '+TRIM(STR(&PedCod2))+'; FATURAMENTO DA NOTA FISCAL: '+TRIM(STR(&SdtNf.NfsNum))+'; SÉRIE: '+TRIM(&SdtNf.NfsSer) +'; PRODUTO: '+TRIM(&MovPrdCod)
                       Call(PLogOperacao, &logon.UsrCod,'INS',&LOGHST)
                    endnew    
                           
                    do'acertastq'
                EndIF    
             endif   
         endif
     EndIf
endfor

&LOGHST = 'NOTA FISCAL DE VENDA: '+TRIM(STR(&SdtNf.NfsNum))+' - '+TRIM(&SdtNf.NfsSer)
Call(PLogOperacao, &logon.UsrCod,'INS',&LOGHST)

&Flag = 'N'
if &pednumecf <> 0 
   do'locCcr'
endif

//movimenta Contas a Receber
if (&PedMovCcr = 1 or &EmpTpMovStqCcr = 'C') and &Flag = 'N' // Pedido movimenta  contas a receber ou empresa controla por cfop se movimenta contas a receber

   &seq = 0
   for &SdtNfParItem in &SdtNfPar

       &seq += 1
              if not null(&SdtNfParItem.FormCod)
                 &PedFormPgt = &SdtNfParItem.FormCod
                 &PedOpeCartao = &SdtNfParItem.OpeSeq
              endif 

              if &PedFormPgt = 4 or &PedFormPgt = 5 or &PedFormPgt = 8 or &PedFormPgt = 9 or &PedFormPgt = 10 or &PedFormPgt = 11 
                 do'buscacartao'
              else
                &OpePrc = 0
                &OpeBanCod = ''
                &OpeAgeCod = 0
                &OpeAgeDig = ''
                &OpeCceCod = ''
                &OpeCceDig = ''
              endif 

              //Grava Contas a Receber
              new   
                 If &Logon.EmpClienteNa12 = 3 // Se for Sacaplast, data de emissão das parcelas será igual emissão do pedido
                     CcrDtaEmi = &PedDta
                 Else
                     CcrDtaEmi = &SdtNf.NfsDtaEms        
                 EndIf 

                 CcrDtaVct = &SdtNfParItem.NfpVct

                 if null(&SdtNfParItem.NfpVlr2)
                    CcrVlr = &SdtNfParItem.NfpVlr
                    &CcrVlr = &SdtNfParItem.NfpVlr
                 else
                    CcrVlr = &SdtNfParItem.NfpVlr2
                    &CcrVlr = &SdtNfParItem.NfpVlr2
                 endif

                 CcrVlrJur = 0
                 CcrVlrDsc = 0
                 CcrCliCod = &SdtNf.NfsCliCod
                 CcrContabil = 0

                 if &SdtNfParItem.NfpVct = &Today and &PedFormPgt = 1
                    CcrSts = 2
                    CcrDtaPgt = &SdtNfParItem.NfpVct
                 else
                    if (&SdtNfParItem.NfpVct = &Today) and (&PedFormPgt = 4 or &PedFormPgt = 5 or &PedFormPgt = 8 or &PedFormPgt = 9 or &PedFormPgt = 10 or &PedFormPgt = 11) and (&EmpNaoBaixaTitCcrCartao <> 1)
                       CcrSts = 2
                       CcrDtaPgt = &SdtNfParItem.NfpVct
                    else
                       CcrSts = 1
                       CcrDtaPgt.SetNull()
                    endif
                 endif

                 If &Logon.EmpClienteNa12 = 9 and &PedCondTp = 'P' // Bill
                       CcrSts = 1
                       CcrDtaPgt.SetNull()
                 EndIf

                 CcrNumDoc = &SdtNfParItem.NfpNumDoc
                 CcrPar = &seq
                 CcrNumPed = &SdtNf.NfsNumPed
                 CcrObs = &SdtNfParItem.OBS
                 CcrUsr = &Logon.UsrCod
                 CcrTms = now()
                 CcrVenCod = &SdtNf.NfsVenCod
                 
                If &SdtNf.PedVenRepCod > 0
                    CcrVenRepCod = &SdtNf.PedVenRepCod
                Else
                    CcrVenRepCod.SetNull()
                EndIf

                 CcrNumEcf = &pednumecf
                 CcrCxCod  = &UsrCxCod

                 if not null(&PedGrpDespCod)
                    CcrGrpDespCod = &PedGrpDespCod
                 else
                    CcrGrpDespCod.SetNull()
                 endif

                 CcrFormCod = &PedFormPgt
                 CcrOpeSeq = &SdtNf.NfsOpeCartao
                 CcrOpePrc = &SdtNf.NfsOpePrc

                 CcrNumNf = &SdtNf.NfsNum
                 CcrNfSer = &SdtNf.NfsSer

                 if not null(&SdtNf.NfsPla1Cod)
                    CcrPla1Cod = &SdtNf.NfsPla1Cod
                 else
                    CcrPla1Cod.SetNull()
                 endif

                 if not null(&SdtNf.NfsPla2Cod)
                    CcrPla2Cod = &SdtNf.NfsPla2Cod
                 else
                    CcrPla2Cod.SetNull()
                 endif

                 if not null(&SdtNf.NfsPla3Cod)
                    CcrPla3Cod = &SdtNf.NfsPla3Cod
                 else
                    CcrPla3Cod.SetNull()
                 endif

                 if not null(&SdtNf.NfsPla4Cod)
                    CcrPla4Cod = &SdtNf.NfsPla4Cod
                 else
                    CcrPla4Cod.SetNull()
                 endif

                 if not null(&SdtNf.NfsPla5Cod)
                    CcrPla5Cod = &SdtNf.NfsPla5Cod
                 else
                    CcrPla5Cod.SetNull()
                 endif

                 if not null(&OpeBanCod) and not null(&OpeAgeCod) and not null(&OpeCceCod)
                    CcrBanCod = &OpeBanCod
                    CcrAgeCod = &OpeAgeCod
                    CcrCceCod = &OpeCceCod
                 else
                    CcrBanCod.SetNull()
                    CcrAgeCod.SetNull()
                    CcrCceCod.SetNull()
                 endif

                 CcrOcoTitCod = 1 // Carteira

                 // 05/07/2019 Adicionado para gravar a base da comissão para começar a calcular a comissão usando esse atributo
                 If &SdtNf.NfsVlrTotNf > 0
                     &Porcentagem = Round( &PedVlrBaseCms / &SdtNf.NfsVlrTotNf ,10)
                     CcrVlrBaseCms = Round( &CcrVlr * &Porcentagem ,2) 
                 Else
                     CcrVlrBaseCms = 0
                 EndIf
                      
                 CcrFechouComisRep = ''
                 CcrFechouComisVen = ''
             endnew

              &CcrSeq = CcrSeq

               if not null(&SdtNf.NfsPla5Cod)
                  new
                      PlaMovSeq = &CcrSeq
                      PlaMovTp = 1
                      PlaMovPla1Cod = &SdtNf.NfsPla1Cod
                      PlaMovPla2Cod = &SdtNf.NfsPla2Cod
                      PlaMovPla3Cod = &SdtNf.NfsPla3Cod
                      PlaMovPla4Cod = &SdtNf.NfsPla4Cod
                      PlaMovPla5Cod = &SdtNf.NfsPla5Cod
                      PlaMovES = 'S'

                      if null(&SdtNfParItem.NfpVlr2)
                         PlaMovVlr = &SdtNfParItem.NfpVlr
                      else
                         PlaMovVlr = &SdtNfParItem.NfpVlr2
                      endif

                      PlaMovDta = &SdtNfParItem.NfpVct
                  endnew
              endif

              &LOGHST = 'CONTAS A RECEBER: '+TRIM(STR(&ccrseq))+' - Conta Número:'+trim(&SdtNfParItem.NfpNumDoc)
              Call(PLogOperacao, &logon.UsrCod,'INS',&LOGHST)

              if &SdtNfParItem.NfpVct = &Today and &PedFormPgt <> 4 and &PedFormPgt <> 5 and &PedFormPgt <> 8 and &PedFormPgt <> 9 and &PedFormPgt <> 10 and &PedFormPgt <> 11 
                                        
                 &PedCod2 = udp(PSequenciais,'Caixa')

                 new
                    CxSeq = &PedCod2
                    CxDta = &SdtNfParItem.NfpVct

                    if null(&SdtNfParItem.NfpVlr2)
                       CxVlr = &SdtNfParItem.NfpVlr
                    else
                       CxVlr = &SdtNfParItem.NfpVlr2
                    endif

                    CxFormCod = &PedFormPgt
                    CxOpeSeq  = &SdtNf.NfsOpeCartao
                    CxGrpDespCod.SetNull()
                    CxHst = &SdtNf.NfsDescCfop+' REFERENTE A NOTA FISCAL Nº '+&SdtNf.NfsNum.ToString()+' do cliente '+&PedCliNom.Trim()
                    CxTp = 'C'
                    CxUsr = &Logon.UsrCod
                    CxTms = NOW()
                    CxTroco = 0
                    CxSeqCcr = &ccrseq

                    if null(&UsrCxCod)
                       CxCaixa = '99'
                    else
                       CxCaixa = &UsrCxCod
                    endif

                    CxTpMov = 0
                                
                    if not null(&SdtNf.NfsPla1Cod)
                       CxPla1Cod = &SdtNf.NfsPla1Cod
                    else
                       CxPla1Cod.SetNull()
                    endif
                                            
                    if not null(&SdtNf.NfsPla2Cod)
                       CxPla2Cod = &SdtNf.NfsPla2Cod
                    else
                       CxPla2Cod.SetNull()
                    endif
                                            
                    if not null(&SdtNf.NfsPla3Cod)
                       CxPla3Cod = &SdtNf.NfsPla3Cod
                    else
                       CxPla3Cod.SetNull()
                    endif
                                            
                    if not null(&SdtNf.NfsPla4Cod)
                       CxPla4Cod = &SdtNf.NfsPla4Cod
                    else
                       CxPla4Cod.SetNull()
                    endif
                                
                    if not null(&SdtNf.NfsPla5Cod)
                       CxPla5Cod = &SdtNf.NfsPla5Cod
                    else
                       CxPla5Cod.SetNull()
                    endif
                                
                  endnew

                  if not null(&EmpPla5CodCcr)

                       new
                          PlaMovSeq = &CcrSeq
                          PlaMovTp = 1
                          PlaMovPla1Cod = &EmpPla1CodCcr
                          PlaMovPla2Cod = &EmpPla2CodCcr
                          PlaMovPla3Cod = &EmpPla3CodCcr
                          PlaMovPla4Cod = &EmpPla4CodCcr
                          PlaMovPla5Cod = &EmpPla5CodCcr
                          PlaMovES = 'E'

                          if null(&SdtNfParItem.NfpVlr2)
                             PlaMovVlr = &SdtNfParItem.NfpVlr
                          else
                             PlaMovVlr = &SdtNfParItem.NfpVlr2
                          endif

                          PlaMovDta = &SdtNfParItem.NfpVct
                        endnew

                  endif

                endif

                if &PedFormPgt = 4 or &PedFormPgt = 5 or &PedFormPgt = 8 or &PedFormPgt = 9 or &PedFormPgt = 10 or &PedFormPgt = 11 

                    do'buscacartao'
                            
                    if not null(&OpeBanCod) and not null(&OpeAgeCod) and not null(&OpeCceCod)
                            
                       if &PedFormPgt = 4 or &PedFormPgt = 9 or &PedFormPgt = 11
                          &MovBanHist = 'Recebimento de Cartao de Debito referente ao pedido Nº '+&SdtNf.NfsNumPed.ToString()+' do Cliente '+&PedCliNom.Trim()
                       else
                          &MovBanHist = 'Recebimento de Cartao de Credito referente ao pedido Nº '+&SdtNf.NfsNumPed.ToString()+' do Cliente '+&PedCliNom.Trim()
                       endif
                            
                       // calcula valor do desconto
                       if &OpePrc > 0
                          &Val = ROUND((&SdtNfParItem.NfpVlr * &OpePrc) / 100,2)     
                       else
                          &Val = 0
                       endif        
                            
                       new
                          MovBanCod = &OpeBanCod
                          MovBanDta = &SdtNfParItem.NfpVct
                          MovBanAgeCod = &OpeAgeCod
                          MovBanAgeDig = &OpeAgeDig
                          MovBanCceCod = &OpeCceCod
                          MovBanCceDig = &OpeCceDig
                          MovBanTp  = 'C'
                          
                          if null(&SdtNfParItem.NfpVlr2)
                             MovBanVlr = &SdtNfParItem.NfpVlr - &Val
                             MovBanVlrOri = &SdtNfParItem.NfpVlr
                          else
                             MovBanVlr = &SdtNfParItem.NfpVlr2 - &Val
                             MovBanVlrOri = &SdtNfParItem.NfpVlr2
                          endif

                          MovBanFormPgt = &PedFormPgt
                          MovBanPedCod = &SdtNf.NfsNumPed
                          MovBanCupom  = 0
                          MovBanNf     = 0
                          MovBanHist   = &MovBanHist
                          MovBanCcrSeq = &ccrseq
                        endnew

                     endif

                endif

       endfor

endif


for each
    where PedCod = &SdtNf.NfsNumPed

    &LOGHST = 'PEDIDO(FINALIZADO): '+TRIM(STR(PedCod))
    Call(PLogOperacao, &logon.UsrCod,'UPD',&LOGHST)

    PedSts = 2
    PedDtaFin = &SdtNf.NfsDtaEms
    PedNfsNum = &SdtNf.NfsNum
    PedNfsSer = &SdtNf.NfsSer

    If Null(PedCreCod)
        PedCreCod = &EmpCreCodContabil
    EndIf
endfor

commit
return


Sub'AcertaStq'
for each
    where PrdCod = &MovPrdCod
     Call(PSaldoPrd, &MovPrdCod,&PrdEstAtu)
     //PCalculaEstMin.Call(&Today,&PrdCod)
     PrdEstAtu = &PrdEstAtu
     msg('Atualizando Estoque do Produto...',status)
endfor
endsub


sub'locest'
&Flag = 'N'
for each
   where MovDocNum = &SdtNf.NfsNumPed
   
   &Flag = 'S'

endfor
endsub

sub'LocCcr'
&Flag = 'N'
for each
   where CcrNumPed = &SdtNf.NfsNumPed
   
   &Flag = 'S'

endfor
EndSub

sub'buscacartao'

&OpePrc = 0
&OpeBanCod = ''
&OpeAgeCod = 0
&OpeAgeDig = ''
&OpeCceCod = ''
&OpeCceDig = ''

for each
   where OpeSeq = &PedOpeCartao

   if &PedFormPgt = 4 or &PedFormPgt = 9 or &PedFormPgt = 11
      &OpePrc = OpePercDeb
      &OpeBanCod = OpeBanCod
      &OpeAgeCod = OpeAgeCod
      &OpeAgeDig = OpeAgeDig
      &OpeCceCod = OpeCceCod
      &OpeCceDig = OpeCceDig
   endif

   if &PedFormPgt = 5 or &PedFormPgt = 8 or &PedFormPgt = 10
      &OpePrc = OpePercCred
      &OpeBanCod = OpeBanCod
      &OpeAgeCod = OpeAgeCod
      &OpeAgeDig = OpeAgeDig
      &OpeCceCod = OpeCceCod
      &OpeCceDig = OpeCceDig
   endif

   

endfor
endsub


Sub 'UltNfRefSeq'
    &NfRefSeq = 1

    For Each (NfRefSeq)
        Where NfsNum = &SdtNf.NfsNum
        Where NfsSer = &SdtNf.NfsSer
            &NfRefSeq = NfRefSeq + 1
            Exit
    EndFor
EndSub


Sub 'Credibilidade'
    for each CreCod
        where CreCod = &PedCreCod
            &CreVlr = CreVlr
    endfor
EndSub

Sub 'VerificaProdBxStq'
    &MovPrdCod = &PrdCod

    For Each PrdCod
        Where PrdCod = &PrdCod
        Where not null(PrdCodBxStq)
            &MovPrdCod = PrdCodBxStq      
    EndFor
EndSub


Sub 'VerificaBaixaComp'
    &PrdBaixaCompVenda = 'N'

    For Each PrdCodComp
        Where PrdCodComp = &PrdCod
        Where PrdBaixaCompVenda = 'S'

            &PrdBaixaCompVenda = 'S'

            For Each CompPrdCod
                // Saida no Estoque

                &PedCod2 = udp(PSequenciais,'Estoque')

                &MovPrdCod = CompPrdCod

                new 
                   MovSeq = &PedCod2
                   MovPrdCod = &MovPrdCod
                   MovQtd = Round( &SdtNfProItem.NfiQtd * CompConsumo ,4)
                   MovDta = &SdtNf.NfsDtaEms
                   MovTp = &MovTp
                   MovVlr = CompPrdCusMed
                   MovDocNum = &SdtNf.NfsNumPed
                   MovUsr = &Logon.UsrCod
                   MovTms = now()
                   MovNfsNum = &SdtNf.NfsNum
                   MovNfsSer = &SdtNf.NfsSer
                   MovObs = &SdtNf.NfsDescCfop
                   MovIdReg = 'V'
                   MovEcfNum = &pednumecf
    
                   &LOGHST = 'MOVIMENTO DE ESTOQUE: '+TRIM(STR(&PedCod2))+'; FATURAMENTO DA NOTA FISCAL: '+TRIM(STR(&SdtNf.NfsNum))+'; SÉRIE: '+TRIM(&SdtNf.NfsSer) +'; PRODUTO COMPOSIÇÃO: '+TRIM(&MovPrdCod)
                   Call(PLogOperacao, &logon.UsrCod,'INS',&LOGHST)
                endnew    
                       
                do'acertastq'
            EndFor
    EndFor
EndSub


Sub 'BuscaTerceiro'
    &TerCod = 0

    For Each 
        Where TerCnpj = &PedCliCnpj when &PedCliTp = 'J'
        Where TerCpf = &PedCliCpf when &PedCliTp = 'F'
        Defined by TerTms
            &TerCod = TerCod
    EndFor

    If &TerCod = 0
        Msg('Favor cadastrar o cliente como terceiro antes de continuar!')
        Return
    EndIf
EndSub
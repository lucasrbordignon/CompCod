
&SdtNfPar.Clear()
&SdtNfPro.Clear()

for each
    where EmpCod = &Logon.EmpCod 
        &EmpSerie = EmpSerie
        &EmpCrt = EmpCRT 
        &EmpPerCredIcms = EmpPerCredIcms
        &EmpUf = EmpUf
        &EmpContEst = EmpContEst
        &EmpTipoLote = EmpTipoLote
        &EmpRamAtv = EmpRamAtv
        &EmpModCalc = EmpModCalc
        &EmpCalcParc = EmpCalcParc
        &EmpDescIncCOFINS = EmpDescIncCOFINS
        &EmpDescIncICMS = EmpDescIncICMS
        &EMPDescIncIPI = EMPDescIncIPI
        &EmpDescIncPIS = EmpDescIncPIS
        &EmpNumOrcNF = EmpNumOrcNF
        &EmpObsPedProdNf = EmpObsPedProdNf
        &EmpCreCodContabil = EmpCreCodContabil
        &EmpClienteNa12 = EmpClienteNa12
        &EmpTpMovStqCcr = EmpTpMovStqCcr
        &EmpDescIcmsBcPisCofins = EmpDescIcmsBcPisCofins
endfor

&serie = padl(trim(&EmpSerie),3,'0')

for each (NfsNum)
    where NfsSer = &serie 
        &EmpUltNfs = NfsNum + 1   
        exit
endfor

if &EmpUltNfs = 0
   for each
      where EmpCod = &Logon.EmpCod
         &EmpUltNfs = EmpUltNfs + 1
   endfor

   if &EmpUltNfs = 0
      &EmpUltNfs = 1
   endif
endif

for each
   where NfsNum = &EmpUltNfs
   where NfsSer = &serie    
       msg('Ja existe a nota fiscal '+&EmpUltNfs.ToString()+' cadastrada no sistema')   
       return
endfor

commit

// Soma o total das parcelas que poderá ser diferente do pedido quando a empresa controlar por CFOP e algumas CFOPs não gerarem contas a receber, daí o valor total desses produtos não soma no valor total das parcelas
&TotalParcelas = 0

for each PedCod
    where PedCod = &PedCod
    defined by PedPrcVlr
        &TotalParcelas += PedPrcVlr
endfor

for each PedCod
  where PedCod = &PedCod

    &PedCreCod      = PedCreCod
    &PedAcr         = PedAcr
    &PedDsc         = PedDsc
    &PedFrete       = PedFrete
    &PedContQtd     = PedContQtd
    &PedTotVlrPro   = PedTotVlrPro
    &PedOrdCompra   = PedOrdCompra
    &PedNumNfDev    = PedNumNFDev
    &PedSerieNfDev  = PedSerieNfDev
    &PedTipTrn      = PedTipTrn
    &PedTipTrnNOP   = PedTipTrnNOP
    &PedNumOrc      = PedNumOrc
    &PedQtdeCaixa   = PedQtdeCaixa
    &PedQtdeKIlos   = PedQtdeKIlos

    // no case de nota de devolução se o cliente não informou o numero da nota fiscal no pedido emite um erro
    if &PedTipTrnNOP = 3 or &PedTipTrnNOP = 4
       if null(PedNumNFDev) and null(PedSerieNfDev)
          Msg('Atenção: Preencha o Campo "Nº da Nota Fiscal de Devolução" no cadastro de Pedidos')
          return
       endif
    endif

    &totdsc     = 0
    &totacr     = 0
    &totfrete   = 0
    &totdsc2    = 0
    &totacr2    = 0
    &totfrete2  = 0
    &PdiOrdCompra = NullValue(&PdiOrdCompra)

    if not null(&PedCreCod) and &PedCreCod <> &EmpCreCodContabil // Se for 100% com nota conforme credibilidade informada nas configurações, não precisa dividir.
        do'DividePed'
    else
       &cont = 0
       for each PedCod, PdiSeq
           defined by PdiQtd

            &cont += 1

            If &PedTotVlrPro > 0
                &Desconto   = Round((&PedDsc   / &PedTotVlrPro * PdiTotVlr),2)
                &Acrescimo  = Round((&PedAcr   / &PedTotVlrPro * PdiTotVlr),2)
                &Frete      = Round((&PedFrete / &PedTotVlrPro * PdiTotVlr),2)
            EndIF

            &totdsc     += &Desconto
            &totacr     += &Acrescimo
            &totfrete   += &Frete

            // Faz ajuste da diferença no último produto
            If &cont = &PedContQtd
                If &PedDsc <> &totdsc
                    &Desconto += &PedDsc - &totdsc
                EndIf

                If &PedAcr <> &totacr
                    &Acrescimo += &PedAcr - &totacr
                EndIf

                If &PedFrete <> &totfrete
                    &Frete += &PedFrete - &totfrete
                EndIf
            EndIf

            &SdtPdi = new SdtPdi()
            &SdtPdi.PdiSeq      = PdiSeq
            &SdtPdi.PdiPrdCod   = PdiPrdCod
            &SdtPdi.PdiPrdDsc   = PdiPrdDsc
            &SdtPdi.PdiVlrUnt   = PdiVlrUntDsc
            &SdtPdi.PdiQtd      = PdiQtd
            &SdtPdi.PdiPrdUnd   = PdiPrdUnd
            &SdtPdi.PdiTotVlr   = PdiTotVlr
            &SdtPdi.PdiFrete    = &Frete
            &SdtPdi.PdiDesconto = &Desconto
            &SdtPdi.PdiAcr      = &Acrescimo
            &SdtPdi.Seq         = 1
            &SdtPdi.PdiCfopSeq  = PdiCfopSeq
            &SdtPdi.PdiCfopCod  = PdiCfopCod
            &SdtPdi.PdiObs      = PdiObs
            &SdtPdi.PdiOrdCompra= PdiOrdCompra
            &SdtPdi.PdiOrdCompraSeq = PdiOrdCompraSeq
            &SdtPdi.PdiCfopMovFin = PdiCfopMovFin
            &SdtPdi.PdiCfopMovStq = PdiCfopMovStq
            &SdtPdi.PdiNfEntrada = PdiNfEntrada
            &ColSdtPdi.Add(&SdtPdi)    
        
            If null(&PdiOrdCompra)
                &OrdCompraItensIgual = 'S'
                &PdiOrdCompra = PdiOrdCompra
            Else
                If &PdiOrdCompra = PdiOrdCompra
                    &OrdCompraItensIgual = 'S'
                Else
                    &OrdCompraItensIgual = 'N'
                EndIf
            EndIf

            If null(&PedOrdCompra)
                &OrdCompraItensIgual = 'N'
            EndIf

       endfor

    endif

endfor

&NfsTotPar = 0

for each PedCod
    where PedCod = &PedCod

    &PedCliCod = PedCliCod
    &PedCliTp  = PedCliTp
    &NfsCliEstado = PedCliUfCod
    &PedCliTp = PedCliTp

    &PedCondCod = PedCondCod
    Do'CondPgtoIPI'

    &PedFormCod = PedFormPgt
    &PedVenCod = PedVenCod
    &PedGrpDespCod = PedGrpDespCod
    &PedPorcCom = PedPorcCom
    &PedMovStq = PedMovStq
    &PedCliIeIse = PedCliIeIse
    &PedMovCcr = PedMovCcr
    &PedDtaBase = PedDtaBase
    &PedCliUfCod = PedCliUfCod
    &PedNumEcf = PedNumECF
    &PedCliEmail = PedCliEmail
    &PedConsFinal = PedConsFinal
    &PedNumOS = PedNumOS
    &PedDta = PedDta
    &PedTotVlr = PedTotVlr
    &PedContQtd = PedContQtd
    &PedTotalQtd = PedTotalQtd
    &PedTotVlrPro = PedTotVlrPro
    &PedTpDsc = PedTpDsc
    &PedObsFis = PedObsFis
    &PedTipTrnContEst = PedTipTrnContEst
    &PedVenRepCod = PedVenRepCod

    //Cond. Pgto Exp.
    &PedCondCodExp = PedCondCodExp
    &PedFormPgtExp = PedFormPgtExp

    &PedCliProdRural = PedCliProdRural
    &PedTipTrn = PedTipTrn
    &SdtNf.NfsFormCod = PedFormPgt

    &SdtNf.MovCcr = &PedMovCcr
    &SdtNf.MovStq = &PedMovStq

    &SdtNf.NfsOpeCartao = PedOpeCartao
    &SdtNf.NfsOpePrc    = PedOpePrc
    &SdtNf.NfsCredibilidade = PedCreCod

    &PedRedespCod = PedRedespCod

    &SdtNf.NfsDescCfop = PedTipTrnDscNF //PedTipTrnDsc Alteração Puff Toys - 06/12/23

    &SdtNf.NfsNum   = &EmpUltNfs
    &SdtNf.NfsSer   = &EmpSerie
    &SdtNf.NfsEmpCod = &Logon.EmpCod
    &SdtNf.NfsCliCod = &PedCliCod
    &SdtNf.NfsVenCod = &PedVenCod
    &SdtNf.PedVenRepCod = &PedVenRepCod
    &SdtNf.NfsDtaEms = &Today
    &SdtNf.NfsDtaSai = &Today
    &SdtNf.PedQtdeCaixa = &PedQtdeCaixa
    &SdtNf.PedQtdeKIlos =  &PedQtdeKIlos

    If Not Null(PedDtaEnt) and (&EmpClienteNa12 = 5 or &EmpClienteNa12 = 6)// Frigolar (ou Cancian): Solicitação da Cibelle 14/01/2020 - Usar a data de entrega do pedido na data de saída da nota fiscal. Obs.: Se não informar data de entrega será usado a data atual. A hora de saída sempre será a atual.
        &SdtNf.NfsDtaSai = PedDtaEnt
    EndIf

    &SdtNf.NfsTpTrn = PedTipTrn  
    &SdtNf.NfsTipTrnNOP = PedTipTrnNOP
    &SdtNf.NfsTpFrt = PedCobraFrete
    &SdtNf.NfsNumPed = &PedCod
    &SdtNf.NfsCondCod = PedCondCod
    &SdtNf.NfsPla1Cod = PedPla1Cod
    &SdtNf.NfsPla2Cod = PedPla2Cod
    &SdtNf.NfsPla3Cod = PedPla3Cod
    &SdtNf.NfsPla4Cod = PedPla4Cod
    &SdtNf.NfsPla5Cod = PedPla5Cod
    &SdtNf.NfsConsFinal = &PedConsFinal
    &SdtNf.NfsTrpCod = PedTrpCod

    do'zonafranca'
    
    &cont = 0

    for &sdtpdi in &ColSdtPdi
        if &SdtPdi.Seq = 1

           &PdiPrdCod = &SdtPdi.PdiPrdCod
    
           if &EmpContEst = 1 // Empresa controla estoque negativo 
              
              &PdiQtd2 = &SdtPdi.PdiQtd
    
              If &EmpTpMovStqCcr = 'P' // Controla Movimento de Estoque e Contas a Receber pelo Pedido
                  If &PedTipTrnContEst = 1 and &PedMovStq = 1
                     Call(PVerificaEst, &Logon,&PdiPrdCod,&PdiQtd2)                  
                  endif
              Else
                 If &PedTipTrnContEst = 1 and &SdtPdi.PdiCfopMovStq = 'S'      
                    Call(PVerificaEst, &Logon,&PdiPrdCod,&PdiQtd2)              
                 endif
              EndIf
    
              if &PdiQtd2 = 0
                 Msg('Operação não pode ser realizada')
                 return
              endif
    
           endif
    
           &PdiQtd3 = 0
    
           &SdtNfProItem = new SdtNfPro.SdtNfProItem()
           &PdiPrdCod = &SdtPdi.PdiPrdCod
           &SdtNfProItem.NfiSeq = &SdtPdi.PdiSeq
           &SdtNfProItem.NfiPrdCod = &SdtPdi.PdiPrdCod
           &SdtNfProItem.NfiPrdDsc =  &SdtPdi.PdiPrdDsc
           &SdtNfProItem.NfiPrdUndCod = &SdtPdi.PdiPrdUnd
           &SdtNfProItem.NfiTpDsc = &PedTpDsc
           &SdtNfProItem.PdiCfopMovFin = &SdtPdi.PdiCfopMovFin
           &SdtNfProItem.PdiCfopMovStq = &SdtPdi.PdiCfopMovStq
           &SdtNfProItem.PdiNfEntrada = &SdtPdi.PdiNfEntrada

           If not null(&SdtPdi.PdiOrdCompra)
                &SdtNfProItem.NfiOrdCompra = &SdtPdi.PdiOrdCompra
           Else
                &SdtNfProItem.NfiOrdCompra = &PedOrdCompra
           EndIf
    
//          &SdtNfProItem.NfiOrdCompraSeq = &SdtPdi.PdiOrdCompraSeq
           
           &PdiQtd = &SdtPdi.PdiQtd
           &PdiVlrUnt = &SdtPdi.PdiVlrUnt
    
           &cont += &SdtPdi.PdiQtd
           &PdiTotVlr2 = 0

           &PdiCfopSeq = &SdtPdi.PdiCfopSeq
           &PdiCfopCod = &SdtPdi.PdiCfopCod

            &es = substr(&PdiCfopCod,1,1)
            
            if &es = '1' or &es = '2' or &es = '3'
               &NfsTpNf = 0
            else
               &NfsTpNf = 1
            endif

            &SdtNf.NfsTpNf = &NfsTpNf

           do'Produto'
               
           &SdtNfProItem.NfiFcI    = &NfiFci
           &SdtNfProItem.NfiIndTot = &CfopIndTot
    
           &PdiTotVlr = &SdtPdi.PdiTotVlr //- &PdiDsc
    
           if &PdiTotVlr2 = 0
              &PdiTotVlr2 = &PdiTotVlr
           endif
    
           &PedFrete = &SdtPdi.PdiFrete
           &PedAcr   = &SdtPdi.PdiAcr
           &PedDsc   = &SdtPdi.PdiDesconto
    
           &SdtNfProItem.NfiTotPrd = &PdiTotVlr

           &SdtNfProItem.NfiObs = ''
                       
           If &EmpObsPedProdNf = 1 // Observação do produto inserida no pedido sai na nota fiscal
               &SdtNfProItem.NfiObs = &SdtPdi.PdiObs.Trim() 
           EndIf

           If not Null(&PrdObs.Trim())
               &SdtNfProItem.NfiObs += '\ ' + &PrdObs.Trim()
           EndIf

           If &OrdCompraItensIgual = 'N' and not null(&SdtNfProItem.NfiOrdCompra) // Se a ordem de compra for diferente em cada item, grava a observação no item e não na nota
               &SdtNfProItem.NfiObs += '\ ' + 'Ord. Compra = ' + trim(&SdtNfProItem.NfiOrdCompra) 
           EndIf
           
           &NfiVlrIpiDev = 0
    
            if &PedTipTrnNOP = 3 or &PedTipTrnNOP = 4 // Calcula IPI devolvido se for Devolução de Venda ou Devolução de Compra ||&PedTipTrn = 2 or &PedTipTrn = 9
                PCalcNfDev.Call(&Logon, &PedNumNfDev,&PedSerieNfDev,&PerDev,&PedTipTrnNOP,&PdiPrdCod,&PdiQtd,&NfiVlrIpiDev) 
               &SdtNfProItem.NfiPerDev = &PerDev
    
           do'CalcIpi'

               &SdtNfProItem.NfiVlrIpiDev = &NfiVlrIpiDev
            endif
    
           do'CalcIcms'
    
           do'CalcPisCof'
    
           if &CidDesCof > 0
              &SdtNfProItem.NfiDesCofins = Round((&PdiTotVlr * &CidDesCof / 100),2)
           endif
    
           if &CidDesIcms > 0
              &SdtNfProItem.NfiDesIcms = Round((&PdiTotVlr * &CidDesIcms / 100),2)
           endif
    
           if &CidDesPis > 0
              &SdtNfProItem.nfidesPis = Round((&PdiTotVlr * &CidDesPis / 100),2)
           endif
    
           if &NcmAlqFedNac > 0 or &NcmAlqFedImp > 0 or &NcmAlqEst > 0 or &NcmAlqMun > 0
              if &CfopLeiOlhoImp = 1 or (&PedConsFinal = 'S' and &CfopLeiOlhoImp = 0)
    
                  &SdtNfProItem.NfiTotTribEst = Round( (&SdtNfProItem.NfiTotPrd * &NcmAlqEst) / 100 ,2)
                  &SdtNfProItem.NfiTotTribMun = Round( (&SdtNfProItem.NfiTotPrd * &NcmAlqMun) / 100 ,2)
    
                  &PerFed = 0
                  if &PrdOriPro = 1 or &PrdOriPro = 2 or &PrdOriPro = 8
                     &PerFed = &NcmAlqFedImp
                     &SdtNfProItem.NfiTotTribFedImp = Round( (&SdtNfProItem.NfiTotPrd * &NcmAlqFedImp) / 100 ,2)
                  else
                     &PerFed = &NcmAlqFedNac
                     &SdtNfProItem.NfiTotTribFedNac = Round( (&SdtNfProItem.NfiTotPrd * &NcmAlqFedNac) / 100 ,2)
                  endif
    
                  &SdtNfProItem.NfiTotTrib = &SdtNfProItem.NfiTotTribEst + &SdtNfProItem.NfiTotTribMun + &SdtNfProItem.NfiTotTribFedImp + &SdtNfProItem.NfiTotTribFedNac
    
              endif
           endif
    
           if &SdtNfProItem.NfiTotTrib > 0
            
                if &SdtNfProItem.NfiTotTribEst > 0 
                   &SdtNfProItem.NfiObs += '\ Total aproximado dos Tributos Estaduais: R$ '+ TRIM(STR(&SdtNfProItem.NfiTotTribEst,10,2))+'('+trim(str(&NcmAlqEst,10,2))+' %)'
                endif
    
                if &SdtNfProItem.NfiTotTribMun > 0 
                   &SdtNfProItem.NfiObs += '\ Total aproximado dos Tributos Municiais: R$ '+ TRIM(STR(&SdtNfProItem.NfiTotTribMun,10,2))+'('+trim(str(&NcmAlqMun,10,2))+' %)'
                endif
    
                if &SdtNfProItem.NfiTotTribFedNac > 0 or &SdtNfProItem.NfiTotTribFedImp > 0
    
                   &TotFed = &SdtNfProItem.NfiTotTribFedNac + &SdtNfProItem.NfiTotTribFedImp  
                   &SdtNfProItem.NfiObs += '\ Total aproximado dos Tributos Federais: R$ '+ TRIM(STR(&TotFed,10,2))+'('+trim(str(&PerFed,10,2))+' %)'
    
                endif
    
              //&SdtNfProItem.NfiObs += '\ Total aproximado de tributos federais,estaduais e municipais: R$ '+trim(str(&SdtNfProItem.NfiTotTrib,10,2))+'('+trim(str(&NcmAlqEcf,10,2))+')'
           endif

           // Criado essas variáveis com nome "Formatado" apenas para concatenar nos dados adicionais, pois se jogar direto a SDT o valor fica cheio de zeros à esquerda
           &NfiBseClcStFormatado = &SdtNfProItem.NfiBseClcSt
           &NfiVlrStFormatado = &SdtNfProItem.NfiVlrSt
    
           if &EmpRamAtv = 2 and &SdtNfProItem.NfiVlrSt > 0  //Comercio não destaca ST
                 &SdtNfProItem.NfiObs += '\ Valor da BC do ICMS ST: R$ '+trim(toformattedstring(&NfiBseClcStFormatado))+' - Valor do ICMS ST: R$ '+trim(toformattedstring(&NfiVlrStFormatado))
                 If &PerSt > 0
                      &SdtNfProItem.NfiObs += ' - MVA: ' + trim(ToFormattedString(&PerSt)) + ' % '
                 EndIf
                 If Not Null(&CestCod) 
                      &SdtNfProItem.NfiObs += ' - CEST: ' + trim(ToFormattedString(&CestCod))  
                 EndIf
                 &SdtNfProItem.NfiOutDesp += &SdtNfProItem.NfiVlrSt // Alterado dia 28/11/2017: Estava atribuindo ao invés de somar
                 &SdtNf.NfsOutDsp += &SdtNfProItem.NfiVlrSt
                 &SdtNfProItem.NfiBseClcSt = 0
                 &SdtNfProItem.NfiVlrSt = 0
           endif

           // Inclusão da Observação do ICMS ST por Item dia 13/01/2020 a pedido da Fátima do Cunha para Frigolar: BASE LEGAL:Parágrafo 5º do Artº 273, do Decreto 45.490/00 do RICMS/00.
           If &SdtNfProItem.NfiVlrSt > 0 and &EmpRamAtv <> 2
                 &SdtNfProItem.NfiObs += '\ Valor da BC do ICMS ST: R$ '+trim(toformattedstring(&NfiBseClcStFormatado))+' - Valor do ICMS ST: R$ '+trim(toformattedstring(&NfiVlrStFormatado))
                 If &PerSt > 0
                      &SdtNfProItem.NfiObs += ' - MVA: ' + trim(ToFormattedString(&PerSt)) + ' % '
                 EndIf
                 If Not Null(&CestCod) 
                      &SdtNfProItem.NfiObs += ' - CEST: ' + trim(ToFormattedString(&CestCod))  
                 EndIf
           EndIf

           If &SdtNfProItem.NfiParcelaIcmsRetido <> 0 
               &SdtNfProItem.NfiObs += '\ Base ICMS Retido: R$ ' + trim(toformattedstring(&SdtNfProItem.NfiBseClcSt060)) + ' Valor ICMS Retido: R$ ' + trim(toformattedstring(&SdtNfProItem.NfiVlrSt060)) + ' Parcela ICMS Retido: R$ ' + trim(toformattedstring(&SdtNfProItem.NfiParcelaIcmsRetido))
           EndIf
    
           &SdtNfProItem.NfiOutDesp += &PedAcr // Alterado dia 28/11/2017: Estava atribuindo ao invés de somar
    
    //       &SdtNfProItem.NfiVlrDsc = &SdtNfProItem.NfiDesCofins + &SdtNfProItem.NfiDesIcms + &SdtNfProItem.nfidesPis
           if &SdtNfProItem.NfiDesIcms = 0
              &SdtNfProItem.NfiVlrDsc = &SdtNfProItem.NfiDesCofins + &SdtNfProItem.nfidesPis + &SdtPdi.PdiDesconto // Alterado dia 30/11/2017: Não estava somando o desconto incondicional junto com o pis e cofins
           else
              &SdtNfProItem.NfiVlrDsc  = &SdtNfProItem.NfiDesCofins + &SdtNfProItem.nfidesPis + &SdtPdi.PdiDesconto
              &SdtNfProItem.NfiIcmsDes = &SdtNfProItem.NfiDesIcms
           endif
    
           &SdtNfProItem.NfiVlrTot = round( (&PdiTotVlr * &CfopIndTot) + &SdtNfProItem.NfiVlrSt + &PedFrete + &SdtNfProItem.NfiOutDesp +&SdtNfProItem.NfiVlrIpi - &SdtNfProItem.NfiDesCofins - &SdtNfProItem.NfiDesIcms - &SdtNfProItem.NfiDesPis - &PedDsc + &SdtNfProItem.NfiVlrFCPSub + &SdtNfProItem.NfiVlrIpiDev,2)

           // &SdtNf.NfsVlrDsc   += &SdtPdi.PdiDesconto // Comentado dia 30/11/2017: Esse desconto passou a ser somado junto com o desconto do pis e cofins

           &SdtNf.NfsVlrTotNf += &SdtNfProItem.NfiVlrTot 

           If &EmpTpMovStqCcr = 'C' // Só irá somar os produto no total das parcelas se a transação estiver marcada para movimentar contas a receber
                If &SdtPdi.PdiCfopMovFin = 'S'

                  If &CondRatIPI = 0
                  	&NfsTotPar += &SdtNfProItem.NfiVlrTot
                  Else
                    &NfsTotPar += &SdtNfProItem.NfiVlrTot - &SdtNfProItem.NfiVlrIpi // Tira o Valor do IPI para depois colocar na primeira parcela
                 	EndIf

                EndIf
           Else
            	If &CondRatIPI = 0
                &NfsTotPar += &SdtNfProItem.NfiVlrTot
              Else
                &NfsTotPar += &SdtNfProItem.NfiVlrTot - &SdtNfProItem.NfiVlrIpi // Tira o Valor do IPI para depois colocar na primeira parcela
              EndIf
           EndIf                    

           &SdtNf.NfsBseClcIcms += &SdtNfProItem.NfiBseClcIcms
           &SdtNf.NfsBseClcSt += &SdtNfProItem.NfiBseClcSt
           &SdtNf.NfsVlrSt += &SdtNfProItem.NfiVlrSt
           &SdtNf.NfsVlrIcms += &SdtNfProItem.NfiVlrIcms
           &SdtNf.NfsDesCofins += &SdtNfProItem.NfiDesCofins
           &SdtNf.NfsDespis += &SdtNfProItem.NfiDesPis
           // &SdtNf.NfsDesIcms += &SdtNfProItem.NfiDesIcms // Alterado dia 07/12/2017: Quando não tem desconto ICMS do suframa usa o desconto do ICMS calculado por CST, quando tem usa o do Suframa
           &SdtNf.NfsDesIcms += &SdtNfProItem.NfiIcmsDes
           &SdtNf.NfsBseClcIpi += &SdtNfProItem.NfiBseClcIPI
           &SdtNf.NfsVlrIpi += &SdtNfProItem.NfiVlrIpi
           &SdtNf.NfsPesoBruto += &NfsPesoBruto
           &SdtNf.NfsPesoliquido += &NfsPesoLiquido
           &SdtNf.NfsVlrFrt += &SdtPdi.PdiFrete
           &SdtNf.NfsAcrescimos += &SdtPdi.PdiAcr
           &SdtNf.NfsOutDsp += &SdtPdi.PdiAcr

           // &SdtNf.NfsVlrDsc += &SdtNfProItem.NfiDesPis + &SdtNfProItem.NfiDesCofins + &SdtNfProItem.NfiVlrDsc // Alterado dia 30/11/2017: Estava duplicando a soma do desconto do pis e cofins
           &SdtNf.NfsVlrDsc += &SdtNfProItem.NfiVlrDsc
    
           &SdtNf.NfsQtd += &NfsQtd
           &SdtNf.NfsCubagem += &NfsCubagem
    
           &SdtNf.NfsVlrPis += &SdtNfProItem.NfiVlrPis
           &SdtNf.NfsVlrCofins += &SdtNfProItem.NfiVlrCof
           &SdtNf.NfsVlrTotPrd += &PdiTotVlr * &CfopIndTot // Quando o campo &CfopIndTot for = 0 (Item não compõe o total da nota), não deve somar o valor do produto no total da nota e dos produtos
           &SdtNf.NfsTotTrib   += &SdtNfProItem.NfiTotTrib
           &SdtNf.NfsTotTribEst += &SdtNfProItem.NfiTotTribEst
           &SdtNf.NfsTotTribFedImp += &SdtNfProItem.NfiTotTribFedImp
           &SdtNf.NfsTotTribFedNac += &SdtNfProItem.NfiTotTribFedNac
           &SdtNf.NfsTotTribMun += &SdtNfProItem.NfiTotTribMun
           &SdtNf.NfsTotCredIcms += &SdtNfProItem.NfiVlrCredIcms  
           
           &SdtNf.NfsVlrFCPSub   += &SdtNfProItem.NfiVlrFCPSub
           &SdtNf.NfsVlrIPIDev   += &SdtNfProItem.NfiVlrIpiDev
    
           &SdtNfProItem.NfiAlqIpi = &NcmPerIpi
           &SdtNfProItem.NfiAlqIcms = &PerIcms
           &SdtNfProItem.NfiPrdNcmCod = &PrdNcmCod
           &SdtNfProItem.NfiVlrFrete = &SdtPdi.PdiFrete

           // &SdtNfProItem.NfiVlrDsc   = &SdtPdi.PdiDesconto // Alterado dia 30/11/2017: Estava sobrescrevendo o desconto do pis e cofins. Por isso &SdtPdi.PdiDesconto passou a ser somado junto com o pis e cofins
           &SdtNfProItem.NfiAcrescimos = &SdtPdi.PdiAcr        
    
           if &EmpCrt = '3'
              &SdtNfProItem.NfiCst = &CfopCstIcms
           else
              &SdtNfProItem.NfiCst = &CfopCsosn
           endif
    
           &SdtNfProItem.NfiCfopCod = &PdiCfopCod
           &SdtNfProItem.NfiCfopSeq = &PdiCfopSeq
           &SdtNfProItem.NfiQtd = &SdtPdi.PdiQtd
           &SdtNfProItem.NfiVlrUnt = &SdtPdi.PdiVlrUnt
    
           // ---------------- PARTILHA DO ICMS ----------------------------------------------------------

           // Operação Interestadual
           if &PedCliUfCod <> &EmpUf
    
              // Cliente não contribuente do estado 
              if &PedCliIeIse = 'N'
    
                 // Pedido é para consumidor final
                 if &PedConsFinal = 'S' 

                    // No cadastro de CFOP está marcado Partilha ICMS = Sim
                    If &CfopPartilha = 'S'                    
    
                        do'partilha'

                        &SdtNf.NfsVlrIcmsDest += &SdtNfProItem.NfiVlrIcmsDest
                        &SdtNf.NfsVlrIcmsOri  += &SdtNfProItem.NfiVlrIcmsOri
                        &SdtNf.NfsVlrFCP += &SdtNfProItem.NfiVlrFCP                           
                    EndIF    
                 endif    
              endif    
           endif

          &SdtNfPro.Add(&SdtNfProItem)

       endif

    endfor
  
    do'obs'    

if (&PedMovCcr = 1 or &EmpTpMovStqCcr = 'C') and &NfsTotPar > 0
          &SdtNfPar.Clear()

          if &EmpCalcParc = 1
             do 'buscaparc'
          else
             do 'parcelas'
          endif

     
          //Se For 'Cobrado 1º Parcela' 
          If &CondRatIPI = 1
        
                //Colocar o valor do IPI na 1 Parcela
                For &SdtNfParItem in &SdtNfPar
                
                   If &SdtNfParItem.NfpSeq = 1            
                       &SdtNfParItem.NfpVlr += &SdtNf.NfsVlrIpi
                       &NfsTotPar           += &SdtNf.NfsVlrIpi   // coloca na variavel o total do IPI que foi Retirado la em cima
                       exit
                   EndIf
                
                EndFor

                /////////////////////////////////////////////
                // FAZ A VERIFICAÇÃO DO TOTAL DAS PARCELAS //
                /////////////////////////////////////////////
              
                &Total2 = 0
                for &SdtNfParItem in &SdtNfPar
                
                    &Total2 += round(&SdtNfParItem.NfpVlr,2)
                
                endfor


                
                if &Total2 <> &NfsTotPar
                    
                       &dif = &NfsTotPar - &Total2
                    
                       for &SdtNfParItem in &SdtNfPar
                    
                           &SdtNfParItem.NfpVlr += &dif
                           exit
                    
                       endfor
                    
                endif



          EndIf

    endif

endfor

do case
  case &ModoImp = 'P1'
     RRelPrePedConf.Call(&Logon, &SdtNf, &SdtNfPar, &SdtNfPro)
  case &ModoImp = 'P2'
     RRelPrePedCli.Call(&Logon, &SdtNf, &SdtNfPar, &SdtNfPro)
  case &ModoImp = 'N'

      &Flag = 'N'
      call(WNotaFiscal,&Logon,&SdtNf,&SdtNfPar,&SdtNfPro,&Flag)
      
      //&Flag = 'N'
      //for each
      //    where NfsNum = &EmpUltNfs
      //    where NfsSer = &serie
      //   
      //    &Flag = 'S'
      //
      //endfor
      
      
      ///////////////////////////////////////////
      //CALCUO DA DAS PARCELA DA PARTE SEM NOTA//
      ///////////////////////////////////////////
      
      if &Flag = 'S'
      
          &cont = 0
          &TotalParcelas = 0
      
      ///  CODIGO ANTIGO ////
          for &SdtPdi in &ColSdtPdi
              if &SdtPdi.Seq = 2
                 &cont += 1
      //           &TotalParcelas += &SdtPdi.PdiTotVlr + &SdtPdi.PdiFrete + &SdtPdi.PdiAcr - &SdtPdi.PdiDesconto
      
      //           If &SdtPdi.PdiCfopMovFin = 'N' // CFOP do produto não movimenta contas a receber
      //               &TotalParcelas -= &SdtPdi.PdiTotVlr
      //           EndIf
                  
              endif
          endfor
      ///  CODIGO ANTIGO ////
      
      
      
      
          
          if &cont > 0
      
      
              //NOVO        
              // Soma o total das parcelas que poderá ser diferente do pedido quando a empresa controlar por CFOP e algumas CFOPs não gerarem contas a receber, daí o valor total desses produtos não soma no valor total das parcelas
              &TotalParcelas = 0
              For Each PedCod
               Where PedCod = &PedCod
                Defined by PedPrcVlr
                      &TotalParcelas += PedPrcVlr
              EndFor
              
              &NfsVlrTotPrd  = 0
              For Each 
               Where NfsNum = &SdtNf.NfsNum
               Where NfsSer = &SdtNf.NfsSer
                Defined by NfsVlrTotPrd
              
                    If  NfsNumPed = &PedCod
                      &NfsVlrTotPrd = NfsVlrTotPrd
                    EndIf
              
              EndFor
              
              //TIRA O VALOR DA MERCADORIA DA NOTA FISCAL 
              &TotalParcelas = &TotalParcelas - &NfsVlrTotPrd        
              //NOVO
          
                // MOVIMENTA ESTOQUE 
                for &SdtPdi in &ColSdtPdi
                    if &SdtPdi.Seq = 2
      
                       &PdiPrdCod = &SdtPdi.PdiPrdCod
                       &PdiQtd    = &SdtPdi.PdiQtd
                       &PdiVlrUnt = &SdtPdi.PdiVlrUnt
      
                       if (&PedMovStq = 1 or (&SdtPdi.PdiCfopMovStq = 'S' and &EmpTpMovStqCcr = 'C') ) and &CreTp = 'Q' // Controla Movimento de Estoque e Contas a Receber pelo Pedido ou CFOP
      
                           &obs       = 'FATURAMENTO REFERENTE AO PEDIDO NUMERO '+Trim(str(&PedCod))
                           
                           if &SdtNf.NfsTpNf = 0
                              &es = 'E'
                           else
                              &es = 'S'
                           endif
          
                           PGravaStq2.Call(&Logon,&PdiPrdCod,&PdiQtd,&ES,&PdiVlrUnt,&obs,&PedCod,&MovSeq,'',0)
      
                       endif
      
                    endif
                endfor
      
                if (&PedMovCcr = 1 or &EmpTpMovStqCcr = 'C') and &TotalParcelas > 0
      
                   if &EmpCalcParc = 1
                         do 'buscaparc2'
                   else
                         do 'parcelas2'
                   endif
      
                   WNFParc.Call(&Logon,&SdtNfPar,&TotalParcelas,&PedCod)
      
                endif
          
          endif
      
      endif
endcase

sub'obs'

     &SdtCfopObs.Clear()
     &SdtObs.Clear()
         
     &SdtNf.NfsInfCmp += 'Pedido = ' + trim(str(&PedCod)) + ' / '

     if not null(&PedOrdCompra) and &OrdCompraItensIgual = 'S' // A ordem de compra é igual em todos os itens de compra                                                                                                                                                                  
         &SdtNf.NfsInfCmp += 'Ord. Compra = ' + trim(&PedOrdCompra) + ' /'                                                                                                                           
     endif 

     if not null(&PedNumOrc) and &EmpNumOrcNF = 1                                                                                                                                                                      
         &SdtNf.NfsInfCmp += 'Orcamento = ' + trim(str(&PedNumOrc)) + ' /'                                                                                                                           
     endif 

     if not null(&SdtNf.NfsCubagem)    
         &SdtNf.NfsInfCmp += 'Cubagem(m3): '+trim(str(&SdtNf.NfsCubagem,10,3))+' / '  
     endif

     if not null(&PedRedespCod)    
         for each
             where TrpCod = &PedRedespCod    
                &SdtNf.NfsInfCmp += 'Redespacho: '+TrpNom.Trim()+' Endereço:'+TrpEnd.Trim()+','+TrpEndNum.Trim()+' - '+TrpEndBai.Trim()+'  '+TrpCidNom.Trim()+' - '+TrpUFCod.Trim() +' / ' 
         endfor    
     endif  

//     if not null(&CliCnpjEndEnt) or Not null(&CliCpfEndEnt) 
//        if not null(&CliEndEnt)    
//            &SdtNf.NfsInfCmp += 'Endereco de Entrega: ' + &CliEndEnt.Trim()+','+&CliEndNumEnt.Trim()+','+&CliEndComplEnt.Trim()+'-'+&CliEndBaiEnt.Trim()+','+&CliCidNomEnt.Trim()+'-'+&CliUfCodEnt.Trim()+' / '
//        endif
//     endif

     If &SdtNfProItem.NfiDesCofins > 0 Or &SdtNfProItem.NfiDesIcms > 0 Or &SdtNfProItem.nfidesPis > 0
         &SdtNf.NfsInfCmp += 'SUFRAMA: ' + Trim(ToformattedString(&CliInsSuf)) + ' / ' + ' Area de Livre Comercio:'
     Endif

     if &SdtNfProItem.NfiDesIcms > 0
        &SdtNf.NfsInfCmp += 'Desconto: ' + trim(str(&CidDesIcms,6,2)) + '% ICMS = ' + TRIM(STR(&SdtNf.NfsDesIcms,12,2)) + ' / '                                                     
     endif

     if &SdtNfProItem.nfidesPis > 0
        &SdtNf.NfsInfCmp += 'Desconto: '+trim(str(&CidDesPis,6,2))+'% PIS = '+TRIM(STR(&SdtNf.NfsDespis,12,2))+' / '                                                                           
     endif  

     if &SdtNfProItem.NfiDesCofins > 0
        &SdtNf.NfsInfCmp += 'Desconto: '+trim(str(&CidDesCof,6,2))+'% COFINS = '+TRIM(STR(&SdtNf.NfsDesCofins,12,2))+' / '                                                                            
     endif  
 
     if &SdtNf.NfsVlrIcmsDest > 0 
        &SdtNf.NfsInfCmp += 'Valores totais do ICMS Interestadual: DIFAL da UF Destino R$ ' + trim(str(&SdtNf.NfsVlrIcmsDest,15,2)) + ' + FCP R$ ' + trim(str(&SdtNf.NfsVlrFCP,15,2))  + '; DIFAL da UF Origem R$ '  + trim(str(&SdtNf.NfsVlrIcmsOri,15,2)) + ' / '
     endif
    
     if &EmpCrt <> '3'
    
           &SdtNf.NfsInfCmp += 'Documento emitido por ME ou EPP, optante pelo Simples Nacional, nao gera direito a credito fiscal de IPI - '
    
           if &SdtNf.NfsTotCredIcms > 0
              &SdtNf.NfsInfCmp += 'Permite o aproveitamento do credito de ICMS no valor de R$ '+TRIM(STR(&SdtNf.NfsTotCredIcms,10,2))+' ,correspondente a aliquota de '+trim(str(&EmpPerCredIcms,10,2))+'%, nos termos do artigo 23 da LC 123/06 ' +' / '
           endif
    
      endif
    
      if not null(&PedObsFis)   
           &SdtNf.NfsInfCmp += &PedObsFis.Trim() + ' / '
      endif
      
      if not null(&CliInfCmp) and &EmpClienteNa12 = 6 // Cancian
           &SdtNf.NfsInfCmp += &CliInfCmp.Trim() + ' / '
      endif
    
      for &SdtNfProItem in &SdtNfPro
    
           &cfopseq = &SdtNfProItem.NfiCfopSeq
    
           &Flag2 = 'N'
           for &SdtCfopObsItem in &SdtCfopObs
              if &SdtCfopObsItem.CfopSeq = &cfopseq
                 &Flag2 = 'S'
              endif
           endfor
    
           if &Flag2 = 'N'
    
              &SdtCfopObsItem = new SdtCfopObs.SdtCfopObsItem()
              &SdtCfopObsItem.CfopSeq = &cfopseq
              
              &CfopObs = ''
              for each
                  where CfopSeq = &cfopseq
                  
                  &CfopObs = CfopObs
    
              endfor
    
              &SdtCfopObsItem.CfopObs = &CfopObs
              &SdtCfopObs.Add(&SdtCfopObsItem)
    
           endif
    
           &prdcod = &SdtNfProItem.NfiPrdCod
           &PrdObsCod = 0
           for each
              where PrdCod = &prdcod
              &PrdObsCod = PrdObsCod
           endfor
    
           &Flag2 = 'N'
           for &SdtObsItem in &SdtObs
               if &SdtObsItem.ObsSeq = &PrdObsCod
                  &Flag2 = 'S'
               endif
           endfor
    
           if &Flag2 = 'N'
    
              &SdtObsItem = new SdtObs.SdtObsItem()
              &SdtObsItem.ObsSeq = &PrdObsCod
    
              for each
                 where ObsSeq = &PrdObsCod
                    &PrdObsDsc = ObsDsc
              endfor
    
              &SdtObsItem.ObsDsc = &PrdObsDsc
              &SdtObs.Add(&SdtObsItem)
    
           endif
      
       endfor
    
       if &SdtObs.Count > 0  or &SdtCfopObs.Count > 0
    
          for &SdtCfopObsItem in &SdtCfopObs
              If not Null(&SdtCfopObsItem.CfopObs)
                   &SdtNf.NfsInfCmp += &SdtCfopObsItem.CfopObs.Trim()+' / '
              EndIf
          endfor
    
          for &SdtObsItem in &SdtObs
              If not Null(&SdtObsItem.ObsDsc)
                  &SdtNf.NfsInfCmp += &SdtObsItem.ObsDsc.Trim()+' / '
              EndIf
          endfor
    
       endif
    
      if &SdtNf.NfsTotTrib > 0
          
           if &SdtNf.NfsTotTribEst > 0
              &SdtNf.NfsInfCmp += 'Total aproximado de Tributos Estaduais: R$ '+trim(str(&SdtNf.NfsTotTribEst,10,2))+' -Fonte IBPT'+' / '
           endif
    
           if &SdtNf.NfsTotTribMun > 0
              &SdtNf.NfsInfCmp += 'Total aproximado de Tributos Municipais: R$ '+trim(str(&SdtNf.NfsTotTribMun,10,2))+' -Fonte IBPT'+' / '
           endif
    
           if &SdtNf.NfsTotTribFedImp > 0 or &SdtNf.NfsTotTribFedNac > 0
              &TotFed = &SdtNf.NfsTotTribFedImp + &SdtNf.NfsTotTribFedNac
              &SdtNf.NfsInfCmp += 'Total aproximado de Tributos Federais: R$ '+trim(str(&TotFed,10,2))+' -Fonte IBPT'+' / '
           endif
    
      endif
    
      if not null(&PedCliEmail)
         &SdtNf.NfsInfCmp += 'Email do Destinatário:'+&PedCliEmail.Trim()
      endif

endsub


sub'buscaParc'

for each
   where CondCod = &PedCondCod

   &CondPorc1 = CondPorc1
   &CondPorc2 = CondPorc2
   &CondPorc3 = CondPorc3
   &CondPorc4 = CondPorc4
   &CondPorc5 = CondPorc5
   &CondPorc6 = CondPorc6
   &CondPorc7 = CondPorc7
   &CondPorc8 = CondPorc8
   &CondPorc9 = CondPorc9
   &CondPorc10 = CondPorc10
   &CondPorc11 = CondPorc11
   &CondPorc12 = CondPorc12
   &CondNumPrc = CondNumPrc

endfor

&SdtNfPar.Clear()
for each
   where PedCod = &PedCod

   if PedPrcSeq = 1

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))      

      //&SdtNfParItem.NfpVlr = round(&SdtNf.NfsVlrTotNf * (&CondPorc1 / 100),2)

      &Porcentagem = Round( PedPrcVlr / &TotalParcelas * 100 ,10)
      &SdtNfParItem.NfpVlr = Round( &NfsTotPar * &Porcentagem / 100 ,2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = PedPrcForm

      &SdtNfPar.Add(&SdtNfParItem)

   endif

   if PedPrcSeq = 2

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      //&SdtNfParItem.NfpVlr = round(&SdtNf.NfsVlrTotNf * (&CondPorc2 / 100),2)

      &Porcentagem = Round( PedPrcVlr / &TotalParcelas * 100 ,10)
      &SdtNfParItem.NfpVlr = Round( &NfsTotPar * &Porcentagem / 100 ,2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = PedPrcForm

      &SdtNfPar.Add(&SdtNfParItem)

   endif

   if PedPrcSeq = 3

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      // &SdtNfParItem.NfpVlr = round(&SdtNf.NfsVlrTotNf * (&CondPorc3 / 100),2)

      &Porcentagem = Round( PedPrcVlr / &TotalParcelas * 100 ,10)
      &SdtNfParItem.NfpVlr = Round( &NfsTotPar * &Porcentagem / 100 ,2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = PedPrcForm

      &SdtNfPar.Add(&SdtNfParItem)

   endif

   if PedPrcSeq = 4

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      // &SdtNfParItem.NfpVlr = round(&SdtNf.NfsVlrTotNf * (&CondPorc4 / 100),2)

      &Porcentagem = Round( PedPrcVlr / &TotalParcelas * 100 ,10)
      &SdtNfParItem.NfpVlr = Round( &NfsTotPar * &Porcentagem / 100 ,2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = PedPrcForm

      &SdtNfPar.Add(&SdtNfParItem)

   endif

   if PedPrcSeq = 5

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      // &SdtNfParItem.NfpVlr = round(&SdtNf.NfsVlrTotNf * (&CondPorc5 / 100),2)

      &Porcentagem = Round( PedPrcVlr / &TotalParcelas * 100 ,10)
      &SdtNfParItem.NfpVlr = Round( &NfsTotPar * &Porcentagem / 100 ,2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = PedPrcForm

      &SdtNfPar.Add(&SdtNfParItem)

   endif

   if PedPrcSeq = 6

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      // &SdtNfParItem.NfpVlr = round(&SdtNf.NfsVlrTotNf * (&CondPorc6 / 100),2)

      &Porcentagem = Round( PedPrcVlr / &TotalParcelas * 100 ,10)
      &SdtNfParItem.NfpVlr = Round( &NfsTotPar * &Porcentagem / 100 ,2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = PedPrcForm

      &SdtNfPar.Add(&SdtNfParItem)

   endif

   if PedPrcSeq = 7

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      // &SdtNfParItem.NfpVlr = round(&SdtNf.NfsVlrTotNf * (&CondPorc7 / 100),2)

      &Porcentagem = Round( PedPrcVlr / &TotalParcelas * 100 ,10)
      &SdtNfParItem.NfpVlr = Round( &NfsTotPar * &Porcentagem / 100 ,2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = PedPrcForm

      &SdtNfPar.Add(&SdtNfParItem)

   endif

   if PedPrcSeq = 8

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      // &SdtNfParItem.NfpVlr = round(&SdtNf.NfsVlrTotNf * (&CondPorc8 / 100),2)

      &Porcentagem = Round( PedPrcVlr / &TotalParcelas * 100 ,10)
      &SdtNfParItem.NfpVlr = Round( &NfsTotPar * &Porcentagem / 100 ,2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = PedPrcForm

      &SdtNfPar.Add(&SdtNfParItem)

   endif

   if PedPrcSeq = 9

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      // &SdtNfParItem.NfpVlr = round(&SdtNf.NfsVlrTotNf * (&CondPorc9 / 100),2)

      &Porcentagem = Round( PedPrcVlr / &TotalParcelas * 100 ,10)
      &SdtNfParItem.NfpVlr = Round( &NfsTotPar * &Porcentagem / 100 ,2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = PedPrcForm

      &SdtNfPar.Add(&SdtNfParItem)

   endif


   if PedPrcSeq = 10

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      //&SdtNfParItem.NfpVlr = round(&SdtNf.NfsVlrTotNf * (&CondPorc10 / 100),2)

      &Porcentagem = Round( PedPrcVlr / &TotalParcelas * 100 ,10)
      &SdtNfParItem.NfpVlr = Round( &NfsTotPar * &Porcentagem / 100 ,2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = PedPrcForm

      &SdtNfPar.Add(&SdtNfParItem)

   endif

   if PedPrcSeq = 11

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      //&SdtNfParItem.NfpVlr = round(&SdtNf.NfsVlrTotNf * (&CondPorc11 / 100),2)

      &Porcentagem = Round( PedPrcVlr / &TotalParcelas * 100 ,10)
      &SdtNfParItem.NfpVlr = Round( &NfsTotPar * &Porcentagem / 100 ,2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = PedPrcForm

      &SdtNfPar.Add(&SdtNfParItem)

   endif


   if PedPrcSeq = 12

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      //&SdtNfParItem.NfpVlr = round(&SdtNf.NfsVlrTotNf * (&CondPorc12 / 100),2)

      &Porcentagem = Round( PedPrcVlr / &TotalParcelas * 100 ,10)
      &SdtNfParItem.NfpVlr = Round( &NfsTotPar * &Porcentagem / 100 ,2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = PedPrcForm

      &SdtNfPar.Add(&SdtNfParItem)

   endif
   

endfor

&Total2 = 0
for &SdtNfParItem in &SdtNfPar

    &Total2 += round(&SdtNfParItem.NfpVlr,2)

endfor

if &Total2 <> &NfsTotPar
    
       &dif = &NfsTotPar - &Total2
    
       for &SdtNfParItem in &SdtNfPar
    
           &SdtNfParItem.NfpVlr += &dif
           exit
    
       endfor
    
endif

endsub



sub'buscaParc2'

//Verifica se Cond. Pagto estiver vazio pega da principal
If Null(&PedCondCodExp)
    &PedCondCodExp = &PedCondCod
EndIf

//Verifica se Forma Pagto estiver vazio pega da principal
If Null(&PedFormPgtExp)
    &PedFormPgtExp = &PedFormCod
EndIf

for each
   where CondCod = &PedCondCodExp

   &CondPorc1 = CondPorc1
   &CondPorc2 = CondPorc2
   &CondPorc3 = CondPorc3
   &CondPorc4 = CondPorc4
   &CondPorc5 = CondPorc5
   &CondPorc6 = CondPorc6
   &CondPorc7 = CondPorc7
   &CondPorc8 = CondPorc8
   &CondPorc9 = CondPorc9
   &CondPorc10 = CondPorc10
   &CondPorc11 = CondPorc11
   &CondPorc12 = CondPorc12
   &CondNumPrc = CondNumPrc

endfor

&SdtNfPar.Clear()
for each
   where PedCod = &PedCod

   if PedPrcSeq = 1

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      &SdtNfParItem.NfpVlr = round(&TotalParcelas * (&CondPorc1 / 100),2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp

      &SdtNfPar.Add(&SdtNfParItem)

   endif

   if PedPrcSeq = 2

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      &SdtNfParItem.NfpVlr = round(&TotalParcelas * (&CondPorc2 / 100),2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp

      &SdtNfPar.Add(&SdtNfParItem)

   endif

   if PedPrcSeq = 3

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      &SdtNfParItem.NfpVlr = round(&TotalParcelas * (&CondPorc3 / 100),2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp

      &SdtNfPar.Add(&SdtNfParItem)

   endif

   if PedPrcSeq = 4

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      &SdtNfParItem.NfpVlr = round(&TotalParcelas * (&CondPorc4 / 100),2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp

      &SdtNfPar.Add(&SdtNfParItem)

   endif

   if PedPrcSeq = 5

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      &SdtNfParItem.NfpVlr = round(&TotalParcelas * (&CondPorc5 / 100),2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp

      &SdtNfPar.Add(&SdtNfParItem)

   endif

   if PedPrcSeq = 6

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      &SdtNfParItem.NfpVlr = round(&TotalParcelas * (&CondPorc6 / 100),2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp

      &SdtNfPar.Add(&SdtNfParItem)

   endif

   if PedPrcSeq = 7

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      &SdtNfParItem.NfpVlr = round(&TotalParcelas * (&CondPorc7 / 100),2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp

      &SdtNfPar.Add(&SdtNfParItem)

   endif

   if PedPrcSeq = 8

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      &SdtNfParItem.NfpVlr = round(&TotalParcelas * (&CondPorc8 / 100),2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp

      &SdtNfPar.Add(&SdtNfParItem)

   endif

   if PedPrcSeq = 9

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      &SdtNfParItem.NfpVlr = round(&TotalParcelas * (&CondPorc9 / 100),2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp

      &SdtNfPar.Add(&SdtNfParItem)

   endif


   if PedPrcSeq = 10

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      &SdtNfParItem.NfpVlr = round(&TotalParcelas * (&CondPorc10 / 100),2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp

      &SdtNfPar.Add(&SdtNfParItem)

   endif

   if PedPrcSeq = 11

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      &SdtNfParItem.NfpVlr = round(&TotalParcelas * (&CondPorc11 / 100),2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp

      &SdtNfPar.Add(&SdtNfParItem)

   endif


   if PedPrcSeq = 12

      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = PedPrcSeq
      &SdtNfParItem.NfpVct = PedPrcDta
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(PedPrcSeq))+'/'+trim(str(&CondNumPrc))

      &SdtNfParItem.NfpVlr = round(&TotalParcelas * (&CondPorc12 / 100),2)

      // Alteração 19/10/2017: OS campos &SdtNfParItem.OpeSeq, &SdtNfParItem.OpePrc e &SdtNfParItem.FormCod foram adicionados aqui pois não estava carregando a forma de pagamento nas pacelas sem nota, por isso dava erro e não gravava as parcelas
      &SdtNfParItem.OpeSeq = PedPrcOpe
      &SdtNfParItem.OpePrc = PedPrcOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp

      &SdtNfPar.Add(&SdtNfParItem)

   endif
   

endfor

&Total2 = 0
for &SdtNfParItem in &SdtNfPar

    &Total2 += round(&SdtNfParItem.NfpVlr,2)

endfor

// if &Total2 <> &SdtNf.NfsVlrTotNf // Alterado dia 29/11/2017: Estava comparando o valor total das parcelas da parte 2 com o valor da nota 1 (&SdtNf.NfsVlrTotNf). Foi alterado para comparar com o valor total do pedido da parte 2
if &Total2 <> &TotalParcelas
                  
       &dif = &TotalParcelas - &Total2

       for &SdtNfParItem in &SdtNfPar
    
           &SdtNfParItem.NfpVlr += &dif
           exit
    
       endfor
    
endif


endsub


sub'parcelas'
&SdtNfPar.Clear()
&seq = 0
for each
   where CondCod = &PedCondCod
  
   &CondNumPrc = CondNumPrc

   if &CondNumPrc >= 1

      &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia1
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia1
      endif

      &SdtNfParItem.NfpVlr = Round(&NfsTotPar * (CondPorc1 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormCod
      &SdtNfPar.Add(&SdtNfParItem)

   endif

   if &CondNumPrc >= 2

       &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia2
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia2
      endif

      &SdtNfParItem.NfpVlr = Round(&NfsTotPar * (CondPorc2 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormCod
      &SdtNfPar.Add(&SdtNfParItem)

   endif


    if &CondNumPrc >= 3

      &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia3
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia3
      endif

      &SdtNfParItem.NfpVlr = Round(&NfsTotPar * (CondPorc3 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormCod
      &SdtNfPar.Add(&SdtNfParItem)


   endif

    if &CondNumPrc >= 4

       &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia4
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia4
      endif

      &SdtNfParItem.NfpVlr = Round(&NfsTotPar * (CondPorc4 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormCod
      &SdtNfPar.Add(&SdtNfParItem)

   endif


    if &CondNumPrc >= 5

      &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia5
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia5
      endif

      &SdtNfParItem.NfpVlr = Round(&NfsTotPar * (CondPorc5 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormCod
      &SdtNfPar.Add(&SdtNfParItem)


   endif


    if &CondNumPrc >= 6

       &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia6
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia6
      endif

      &SdtNfParItem.NfpVlr = Round(&NfsTotPar * (CondPorc6 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormCod
      &SdtNfPar.Add(&SdtNfParItem)

   endif

    if &CondNumPrc >= 7

      &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia7
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia7
      endif

      &SdtNfParItem.NfpVlr = Round(&NfsTotPar * (CondPorc7 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormCod
      &SdtNfPar.Add(&SdtNfParItem)


   endif

    if &CondNumPrc >= 8

      &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia8
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia8
      endif

      &SdtNfParItem.NfpVlr = Round(&NfsTotPar * (CondPorc8 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormCod
      &SdtNfPar.Add(&SdtNfParItem)


   endif

    if &CondNumPrc >= 9

      &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia9
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia9
      endif

      &SdtNfParItem.NfpVlr = Round(&NfsTotPar * (CondPorc9 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormCod
      &SdtNfPar.Add(&SdtNfParItem)


   endif

    if &CondNumPrc >= 10

      &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia10
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia10
      endif

      &SdtNfParItem.NfpVlr = Round(&NfsTotPar * (CondPorc10 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormCod
      &SdtNfPar.Add(&SdtNfParItem)

   endif

    if &CondNumPrc >= 11

      &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia11
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia11
      endif

      &SdtNfParItem.NfpVlr = Round(&NfsTotPar * (CondPorc11 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormCod
      &SdtNfPar.Add(&SdtNfParItem)


   endif


    if &CondNumPrc >= 12

      &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia12
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia12
      endif

      &SdtNfParItem.NfpVlr = Round(&NfsTotPar * (CondPorc12 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&EmpUltNfs))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormCod
      &SdtNfPar.Add(&SdtNfParItem)

   endif
   

endfor

&Total2 = 0
for &SdtNfParItem in &SdtNfPar

    &Total2 += round(&SdtNfParItem.NfpVlr,2)

endfor

if &Total2 <> &NfsTotPar
    
       &dif = &NfsTotPar - &Total2
    
       for &SdtNfParItem in &SdtNfPar
    
           &SdtNfParItem.NfpVlr += &dif
           exit
    
       endfor
    
endif

endsub



sub'parcelas2'
&SdtNfPar.Clear()
&seq = 0

//Verifica se Cond. Pagto estiver vazio pega da principal
If Null(&PedCondCodExp)
    &PedCondCodExp = &PedCondCod
EndIf

//Verifica se Forma de Pgto estiver vazio pega da principal
If Null(&PedFormPgtExp)
    &PedFormPgtExp = &PedFormCod
EndIf


for each
   where CondCod = &PedCondCodExp
  
   &CondNumPrc = CondNumPrc

   if &CondNumPrc >= 1

      &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia1
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia1
      endif

      &SdtNfParItem.NfpVlr = Round(&TotalParcelas * (CondPorc1 / 100),2)

      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp
      &SdtNfPar.Add(&SdtNfParItem)

   endif

   if &CondNumPrc >= 2

       &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia2
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia2
      endif

      &SdtNfParItem.NfpVlr = Round(&TotalParcelas * (CondPorc2 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp
      &SdtNfPar.Add(&SdtNfParItem)

   endif


    if &CondNumPrc >= 3

      &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia3
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia3
      endif

      &SdtNfParItem.NfpVlr = Round(&TotalParcelas * (CondPorc3 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp
      &SdtNfPar.Add(&SdtNfParItem)


   endif

    if &CondNumPrc >= 4

       &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia4
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia4
      endif

      &SdtNfParItem.NfpVlr = Round(&TotalParcelas * (CondPorc4 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp
      &SdtNfPar.Add(&SdtNfParItem)

   endif


    if &CondNumPrc >= 5

      &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia5
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia5
      endif

      &SdtNfParItem.NfpVlr = Round(&TotalParcelas * (CondPorc5 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp
      &SdtNfPar.Add(&SdtNfParItem)


   endif


    if &CondNumPrc >= 6

       &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia6
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia6
      endif

      &SdtNfParItem.NfpVlr = Round(&TotalParcelas * (CondPorc6 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp
      &SdtNfPar.Add(&SdtNfParItem)

   endif

    if &CondNumPrc >= 7

      &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia7
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia7
      endif

      &SdtNfParItem.NfpVlr = Round(&TotalParcelas * (CondPorc7 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp
      &SdtNfPar.Add(&SdtNfParItem)


   endif

    if &CondNumPrc >= 8

      &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia8
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia8
      endif

      &SdtNfParItem.NfpVlr = Round(&TotalParcelas * (CondPorc8 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp
      &SdtNfPar.Add(&SdtNfParItem)


   endif

    if &CondNumPrc >= 9

      &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia9
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia9
      endif

      &SdtNfParItem.NfpVlr = Round(&TotalParcelas * (CondPorc9 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp
      &SdtNfPar.Add(&SdtNfParItem)


   endif

    if &CondNumPrc >= 10

      &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia10
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia10
      endif

      &SdtNfParItem.NfpVlr = Round(&TotalParcelas * (CondPorc10 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp
      &SdtNfPar.Add(&SdtNfParItem)

   endif

    if &CondNumPrc >= 11

      &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia11
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia11
      endif

      &SdtNfParItem.NfpVlr = Round(&TotalParcelas * (CondPorc11 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp
      &SdtNfPar.Add(&SdtNfParItem)


   endif


    if &CondNumPrc >= 12

      &seq += 1
      &SdtNfParItem = new SdtNfPar.SdtNfParItem()
      &SdtNfParItem.NfpSeq = &seq

      if null(&PedDtaBase)
         &SdtNfParItem.NfpVct = &Today + CondDia12
      else
         &SdtNfParItem.NfpVct = &PedDtaBase + CondDia12
      endif

      &SdtNfParItem.NfpVlr = Round(&TotalParcelas * (CondPorc12 / 100),2)
      &SdtNfParItem.NfpNumDoc = trim(str(&PedCod))+'-'+trim(str(&seq))+'/'+trim(str(&CondNumPrc))
      &SdtNfParItem.OpeSeq = &SdtNf.NfsOpeCartao
      &SdtNfParItem.OpePrc = &SdtNf.NfsOpePrc
      &SdtNfParItem.FormCod = &PedFormPgtExp
      &SdtNfPar.Add(&SdtNfParItem)

   endif
   

endfor

&Total2 = 0
for &SdtNfParItem in &SdtNfPar

    &Total2 += round(&SdtNfParItem.NfpVlr,2)

endfor

if &Total2 <> &TotalParcelas
    
       &dif = &TotalParcelas - &Total2
    
       for &SdtNfParItem in &SdtNfPar
    
           &SdtNfParItem.NfpVlr += &dif
           exit
    
       endfor
    
endif

endsub


sub'Produto'

    &PerSt = 0
    &NfsPesoBruto = 0
    &NfsPesoLiquido = 0
    &CfopCstIPI = ''
    &CfopCstIcms = ''
    &CfopCsosn = ''
    
    for each
       where PrdCod = &PdiPrdCod       
    
       &NfsPesoLiquido = &PdiQtd * PrdPesoLiq
    
       &NfsPesoBruto = &PdiQtd * PrdPeso // Alteração 13/03/2020: Quando o produto tinha composição, ignorava o peso bruto do produto e calculado a partir do consumo x peso bruto de todos os produtos da composição, porém ninguém tem o peso certinho em toda a composição (Versão 1.3.0)

       If &Logon.EmpClienteNa12 = 6 and PrdUndCod = 'KG' // Cancian: Quando for Cancian e a unidade do produto for KG, não multiplicar o peso pela quantidade pois o peso será igual a quantidade
           &NfsPesoLiquido = &PdiQtd        
           &NfsPesoBruto = &PdiQtd
       EndIf
    
       &PrdEmbCod = PrdEmbCod
       &PrdMtlEmb = PrdMtlEmb
       do'Cubagem'
    
       &PrdObs = PrdObs       
                     
       &PrdOriPro = PrdOriPro
    
       &NfiFci = PrdFCI   
    
       do case
           case &PedCliUfCod = 'AC'
              &PerSt = PrdAcMva
           case &PedCliUfCod = 'AL'
              &PerSt = PrdAlMva
           case &PedCliUfCod = 'AP'
              &PerSt = PrdApMva
           case &PedCliUfCod = 'AM'
              &PerSt = PrdAmMva
           case &PedCliUfCod = 'BA'
              &PerSt = PrdBaMva
           case &PedCliUfCod = 'CE'
              &PerSt = PrdCeMva
           case &PedCliUfCod = 'DF'
              &PerSt = PrdDfMva
           case &PedCliUfCod = 'ES'
              &PerSt = PrdEsMva
           case &PedCliUfCod = 'GO'
              &PerSt = PrdGoMva
           case &PedCliUfCod = 'MG'
              &PerSt = PrdMgMva
           case &PedCliUfCod = 'MT'
              &PerSt = PrdMtMva
           case &PedCliUfCod = 'MS'
              &PerSt = PrdMsMva
           case &PedCliUfCod = 'PA'
              &PerSt = PrdPaMva
           case &PedCliUfCod = 'PB'
              &PerSt = PrdPbMva
           case &PedCliUfCod = 'PR'
              &PerSt = PrdPrMva
           case &PedCliUfCod = 'PI'
              &PerSt = PrdPiMva
           case &PedCliUfCod = 'RS'
              &PerSt = PrdRsMva
           case &PedCliUfCod = 'RN'
              &PerSt = PrdRnMva
           case &PedCliUfCod = 'RR'
              &PerSt = PrdRrMva
           case &PedCliUfCod = 'RO'
              &PerSt = PrdRoMva
           case &PedCliUfCod = 'RJ'
              &PerSt = PrdRjMva
           case &PedCliUfCod = 'SC'
              &PerSt = PrdScMva
           case &PedCliUfCod = 'SP'
              &PerSt = PrdSpMva
           case &PedCliUfCod = 'TO'
              &PerSt = PrdToMva
           case &PedCliUfCod = 'SE'
              &PerSt = PrdSeMva
       endcase               
    
       do'CFOP'

       &PrdNcmCod = PrdNcmCod
       &PrdNcmEx  = PrdNcmEx
       do'NCM'
    
    endfor
endsub


sub'Ncm'

    &NcmPerIpi = 0
    &NcmPerPis = 0
    &NcmPerCof = 0
    &CestCod = NullValue(&CestCod)
    
    &NcmAlqFedNac = 0
    &NcmAlqFedImp = 0
    &NcmAlqEst    = 0
    &NcmAlqMun    = 0

    
    
    for each
       where NcmCod = &PrdNcmCod
       where NcmEx  = &PrdNcmEx
    
       &NcmPerIpi = NcmPerIpi
       &NcmPerPis = NcmPerPis
       &NcmPerCof = NcmPerCof

       &CestCod   = CestCod
      
       &NcmAlqFedNac = NcmAlqFedNac
       &NcmAlqFedImp = NcmAlqFedImp
       &NcmAlqEst    = NcmAlqEst
       &NcmAlqMun    = NcmAlqMun
    
       // % FCP (Fundo de Combate a Pobreza) fica no NCM
       do case
           case &PedCliUfCod = 'AC'
              &SdtNfProItem.NfiPerFCP = NcmFcpAC
           case &PedCliUfCod = 'AL'
              &SdtNfProItem.NfiPerFCP = NcmFcpAL
           case &PedCliUfCod = 'AM'
              &SdtNfProItem.NfiPerFCP = NcmFcpAM
           case &PedCliUfCod = 'AP'
              &SdtNfProItem.NfiPerFCP = NcmFcpAP       
           case &PedCliUfCod = 'BA'
              &SdtNfProItem.NfiPerFCP = NcmFcpBA
           case &PedCliUfCod = 'CE'
              &SdtNfProItem.NfiPerFCP = NcmFcpCE
           case &PedCliUfCod = 'DF'
              &SdtNfProItem.NfiPerFCP = NcmFcpDF
           case &PedCliUfCod = 'ES'
              &SdtNfProItem.NfiPerFCP = NcmFcpES
           case &PedCliUfCod = 'GO'
              &SdtNfProItem.NfiPerFCP = NcmFcpGO
           case &PedCliUfCod = 'MA'
              &SdtNfProItem.NfiPerFCP = NcmFcpMA
           case &PedCliUfCod = 'MG'
              &SdtNfProItem.NfiPerFCP = NcmFcpMG
           case &PedCliUfCod = 'MS'
              &SdtNfProItem.NfiPerFCP = NcmFcpMS
           case &PedCliUfCod = 'MT'
              &SdtNfProItem.NfiPerFCP = NcmFcpMT
           case &PedCliUfCod = 'PA'
              &SdtNfProItem.NfiPerFCP = NcmFcpPA
           case &PedCliUfCod = 'PB'
              &SdtNfProItem.NfiPerFCP = NcmFcpPB
           case &PedCliUfCod = 'PE'
              &SdtNfProItem.NfiPerFCP = NcmFcpPE
           case &PedCliUfCod = 'PR'
              &SdtNfProItem.NfiPerFCP = NcmFcpPR
           case &PedCliUfCod = 'PI'
              &SdtNfProItem.NfiPerFCP = NcmFcpPI
           case &PedCliUfCod = 'RS'
              &SdtNfProItem.NfiPerFCP = NcmFcpRS
           case &PedCliUfCod = 'RN'
              &SdtNfProItem.NfiPerFCP = NcmFcpRN
           case &PedCliUfCod = 'RR'
              &SdtNfProItem.NfiPerFCP = NcmFcpRR
           case &PedCliUfCod = 'RO'
              &SdtNfProItem.NfiPerFCP = NcmFcpRO
           case &PedCliUfCod = 'RJ'
              &SdtNfProItem.NfiPerFCP = NcmFcpRJ
           case &PedCliUfCod = 'SC'
              &SdtNfProItem.NfiPerFCP = NcmFcpSC
           case &PedCliUfCod = 'SP'
              &SdtNfProItem.NfiPerFCP = NcmFcpSP
           case &PedCliUfCod = 'TO'
              &SdtNfProItem.NfiPerFCP = NcmFcpTO
           case &PedCliUfCod = 'SE'
              &SdtNfProItem.NfiPerFCP = NcmFcpSE
        endcase 
    endfor

endsub


Sub'CalcPisCof'
    &SdtNfProItem.NfiCstPis = &CfopCstPis
    &SdtNfProItem.NfiAlqCof = &NcmPerCof
    &SdtNfProItem.NfiAlqPis = &NcmPerPis
    
    if &CfopCstPis = '01' or &CfopCstPis = '02' or &CfopCstPis = '03' or &CfopCstPis = '04' or &CfopCstPis = '05' or &CfopCstPis = '49' or &CfopCstPis = '98' or &CfopCstPis = '99'
        
        &SdtNfProItem.NfiBseClcPIS = (&PdiTotVlr + &PedAcr + &PedFrete)

        if &PedTpDsc = 2 and &EmpDescIncPIS = 1 // Alteração dia 24/11/17: Desconto incondicional desconta na base de cálculo do PIS (se estiver marcado a opção nos parâmetros da empresa)
            &SdtNfProItem.NfiBseClcPIS -= round(&PedDsc,2)
        endif

        if &EmpDescIcmsBcPisCofins = 1 // Alteração dia 29/09/2021: RE N° 574.706 DO STF -  Exclusão do valor do ICMS da Base de Cálculo do PIS e COFINS (se estiver marcado a opção nos parâmetros da empresa)
            &SdtNfProItem.NfiBseClcPIS -= &NfiVlrIcms
        endif

        &SdtNfProItem.NfiBseClcPIS = Round(&SdtNfProItem.NfiBseClcPIS * &CfopBsePis / 100 ,2) // Alteração dia 27/03/2019: Acrescentado base de cálculo do pis para permitir reduzir a base de cálculo quando necessário
        &SdtNfProItem.NfiVlrPis = Round(&SdtNfProItem.NfiBseClcPIS * &NcmPerPis / 100 ,2)       
    else
       &SdtNfProItem.NfiBseClcPIS = 0
       &SdtNfProItem.NfiVlrPis = 0
    endif
    
    &SdtNfProItem.NfiCstCof = &CfopCstCof
    
    if &CfopCstCof = '01' or &CfopCstCof = '02' or &CfopCstCof = '03' or &CfopCstCof = '04' or &CfopCstCof = '05' or &CfopCstCof = '49' or &CfopCstCof = '98' or &CfopCstCof = '99'    

        &SdtNfProItem.NfiBseClcCOFINS = (&PdiTotVlr + &PedAcr + &PedFrete)

        if &PedTpDsc = 2 and &EmpDescIncCOFINS = 1 // Alteração dia 24/11/17: Desconto incondicional desconta na base de cálculo do COFINS (se estiver marcado a opção nos parâmetros da empresa)
            &SdtNfProItem.NfiBseClcCOFINS -= round(&PedDsc,2)
        endif

        if &EmpDescIcmsBcPisCofins = 1 // Alteração dia 29/09/2021: RE N° 574.706 DO STF -  Exclusão do valor do ICMS da Base de Cálculo do PIS e COFINS (se estiver marcado a opção nos parâmetros da empresa)
            &SdtNfProItem.NfiBseClcCOFINS -= &NfiVlrIcms
        endif

        &SdtNfProItem.NfiBseClcCOFINS = Round(&SdtNfProItem.NfiBseClcCOFINS * &CfopBseCofins / 100,2) // Alteração dia 27/03/2019: Acrescentado base de cálculo do cofins para permitir reduzir a base de cálculo quando necessário
        &SdtNfProItem.NfiVlrCof = Round(&SdtNfProItem.NfiBseClcCOFINS * &NcmPerCof / 100,2)
    else
       &SdtNfProItem.NfiBseClcCOFINS = 0
       &SdtNfProItem.NfiVlrCof = 0
    endif
endsub


sub'CFOP'
    &CfopTribIPI = ''
    &CfopBseIcms = 0
    &CfopBseIcmsSt   = 0
    &CfopCstPis  = ''
    &PerIcms = 0
    &CfopCstCof  = ''
    &PerAliqInt = 0
    &CfopPerDiferimento = 0
    &CfopBseCofins = 0
    &CfopBsePis = 0
    
    For each CfopSeq
        where CfopSeq = &PdiCfopSeq
    
        &CfopTribIPI    = CfopTribIpi
        &CfopBseIcms    = CfopBseIcms
        &CfopBseIcmsSt  = CfopBseIcmsSt
        &CfopCstPis     = CfopCstPis
        &CfopCstCof     = CfopCstCof
        &CfopCstIPI     = CfopCstIPI
        &CfopCstIcms    = CfopCstIcms
        &CfopCsosn      = CfopCsosn 
        &CfopIndTot     = CfopIndTot
        &CfopPartilha   = CfopPartilha
        &CfopPerDiferimento = CfopPerDiferimento
        &CfopLeiOlhoImp = CfopLeiOlhoImp
        &CfopBseCofins  = CfopBseCofins
        &CfopBsePis     = CfopBsePis
        &CfopMotDesoneracao = CfopMotDesoneracao
    
        do case
           case &PedCliUfCod = 'AC'
              &PerIcms = CfopAcIcms
           case &PedCliUfCod = 'AL'
              &PerIcms = CfopAlIcms
           case &PedCliUfCod = 'AP'
              &PerIcms = CfopApIcms
           case &PedCliUfCod = 'AM'
              &PerIcms = CfopAmIcms
           case &PedCliUfCod = 'BA'
              &PerIcms = CfopBaIcms
           case &PedCliUfCod = 'CE'
              &PerIcms = CfopCeIcms
           case &PedCliUfCod = 'DF'
              &PerIcms = CfopDfIcms
           case &PedCliUfCod = 'ES'
              &PerIcms = CfopEsIcms
           case &PedCliUfCod = 'GO'
              &PerIcms = CfopGoIcms
           case &PedCliUfCod = 'MG'
              &PerIcms = CfopMgIcms
           case &PedCliUfCod = 'MA'
              &PerIcms = CfopMaIcms
           case &PedCliUfCod = 'MT'
              &PerIcms = CfopMtIcms
           case &PedCliUfCod = 'MS'
              &PerIcms = CfopMsIcms
           case &PedCliUfCod = 'PA'
              &PerIcms = CfopPaIcms
           case &PedCliUfCod = 'PB'
              &PerIcms = CfopPbIcms
           case &PedCliUfCod = 'PE'
              &PerIcms = CfopPeIcms
           case &PedCliUfCod = 'PR'
              &PerIcms = CfopPrIcms
           case &PedCliUfCod = 'PI'
              &PerIcms = CfopPiIcms
           case &PedCliUfCod = 'RS'
              &PerIcms = CfopRsIcms
           case &PedCliUfCod = 'RN'
              &PerIcms = CfopRnIcms
           case &PedCliUfCod = 'RR'
              &PerIcms = CfopRrIcms
           case &PedCliUfCod = 'RO'
              &PerIcms = CfopRoIcms
           case &PedCliUfCod = 'RJ'
              &PerIcms = CfopRjIcms
           case &PedCliUfCod = 'SC'
              &PerIcms = CfopScIcms
           case &PedCliUfCod = 'SP'
              &PerIcms = CfopSpIcms
           case &PedCliUfCod = 'TO'
              &PerIcms = CfopToIcms
           case &PedCliUfCod = 'SE'
              &PerIcms = CfopSeIcms
          endcase
    
          do case
           case &PedCliUfCod = 'AC'
              &PerAliqInt = CfopAcAliq
           case &PedCliUfCod = 'AL'
              &PerAliqInt = CfopAlAliq
           case &PedCliUfCod = 'AM'
              &PerAliqInt = CfopAmAliq
           case &PedCliUfCod = 'AP'
              &PerAliqInt = CfopApAliq           
           case &PedCliUfCod = 'BA'
              &PerAliqInt = CfopBaAliq    
           case &PedCliUfCod = 'CE'
              &PerAliqInt = CfopCeAliq    
           case &PedCliUfCod = 'DF'
              &PerAliqInt = CfopDfAliq    
           case &PedCliUfCod = 'ES'
              &PerAliqInt = CfopEsAliq    
           case &PedCliUfCod = 'GO'
              &PerAliqInt = CfopGoAliq    
           case &PedCliUfCod = 'MA'
              &PerAliqInt = CfopMaAliq
           case &PedCliUfCod = 'MG'
              &PerAliqInt = CfopMgAliq
           case &PedCliUfCod = 'MS'
              &PerAliqInt = CfopMsAliq
           case &PedCliUfCod = 'MT'
              &PerAliqInt = CfopMtAliq
           case &PedCliUfCod = 'PA'
              &PerAliqInt = CfopPaAliq
           case &PedCliUfCod = 'PB'
              &PerAliqInt = CfopPbAliq
           case &PedCliUfCod = 'PE'
              &PerAliqInt = CfopPeAliq
           case &PedCliUfCod = 'PR'
              &PerAliqInt = CfopPrAliq
           case &PedCliUfCod = 'PI'
              &PerAliqInt = CfopPiAliq
           case &PedCliUfCod = 'RS'
              &PerAliqInt = CfopRsAliq
           case &PedCliUfCod = 'RN'
              &PerAliqInt = CfopRnAliq
           case &PedCliUfCod = 'RR'
              &PerAliqInt = CfopRrAliq
           case &PedCliUfCod = 'RO'
              &PerAliqInt = CfopRoAliq
           case &PedCliUfCod = 'RJ'
              &PerAliqInt = CfopRjAliq
           case &PedCliUfCod = 'SC'
              &PerAliqInt = CfopScAliq
           case &PedCliUfCod = 'SP'
              &PerAliqInt = CfopSpAliq
           case &PedCliUfCod = 'TO'
              &PerAliqInt = CfopToAliq
           case &PedCliUfCod = 'SE'
              &PerAliqInt = CfopSeAliq
          endcase                 
    endfor
endsub


sub'CalcIpi'
    &NfiVlrIpi = 0
    &NfiBseClcIPI = 0
    &SdtNfProItem.NfiBseClcIPI = 0
    &SdtNfProItem.NfiVlrIPI = 0
    &SdtNfProItem.NfiAlqIpi = 0
    &SdtNfProItem.NfiCstIpi = &CfopCstIPI

    if &CfopTribIPI = 'S'    
    
       if &CfopCstIPI = '00' or &CfopCstIPI = '50' or &CfopCstIPI = '49'  or &CfopCstIPI = '99'       
    
            if &PedTpDsc = 2 and &EmpDescIncIPI = 1 // Alteração dia 24/11/17: Desconto incondicional desconta na base de cálculo do IPI (se estiver marcado a opção nos parâmetros da empresa). Não estava somando o acréscimo &PedAcr para pessoa jurídica, alterado para somar.
                 if &PedCliTp = 'F'
                    &NfiBseClcIPI = (&PdiTotVlr + &PedFrete + &PedAcr - round(&PedDsc,2))
                    &NfiVlrIpi = round(&NfiBseClcIPI * &NcmPerIpi /100,2)
                 else
                    &NfiBseClcIPI = (&PdiTotVlr + &PedAcr - round(&PedDsc,2))
                    &NfiVlrIpi = round(&NfiBseClcIPI * &NcmPerIpi /100,2)
                 endif
            else
                 if &PedCliTp = 'F'
                    &NfiBseClcIPI = (&PdiTotVlr + &PedFrete + &PedAcr)
                    &NfiVlrIpi = round(&NfiBseClcIPI * &NcmPerIpi /100,2)
                 else
                    &NfiBseClcIPI = (&PdiTotVlr + &PedAcr)
                    &NfiVlrIpi = round(&NfiBseClcIPI * &NcmPerIpi /100,2)
                 endif
            endif
    
            &SdtNfProItem.NfiBseClcIPI = &NfiBseClcIPI
            &SdtNfProItem.NfiVlrIpi = &NfiVlrIpi
       endif                
    
    endif
endsub


sub'CalcIcms'

    &NfiBseClcIcms = 0
    &NfiBseClcSt = 0
    &NfiVlrIcms = 0
    &NfiVlrSt = 0  
         
    // Base de Cálculo do ICMS
    &NfiBseClcIcms  = &PdiTotVlr
    &NfiBseClcIcms += round(&PedFrete,2)
    &NfiBseClcIcms += round(&PedAcr,2)
    if &PedTpDsc = 2 and &EmpDescIncICMS = 1 // Alteração dia 24/11/17: Desconto incondicional desconta na base de cálculo do ICMS (se estiver marcado a opção nos parâmetros da empresa)
        &NfiBseClcIcms -= round(&PedDsc,2)
    endif   
    
    // Base de Cálculo do ICMS ST
    &NfiBseClcIcmsST = &NfiBseClcIcms + &NfiVlrIpi + &NfiVlrIpiDev
                       
    if &PedConsFinal = 'S'
      &NfiBseClcIcms += &NfiVlrIpi + &NfiVlrIpiDev
    endif
    
    // Base de Cálculo Integral do ICMS
    &NfiBseClcIcmsInt = &NfiBseClcIcms
    
    // Valor do ICMS
    &NfiVlrIcms = &NfiBseClcIcms * &PerIcms / 100

    if &EmpCrt = '3' // Regime Normal
     
       do case
          case &CfopCstIcms = '00' // Tributada Integralmente              

               if &NfiVlrIcms > 0
                  &SdtNfProItem.NfiBseClcIcms   = &NfiBseClcIcms
                  &SdtNfProItem.NfiVlrIcms      = &NfiVlrIcms
               else
                  &SdtNfProItem.NfiBseClcIcms   = 0
                  &SdtNfProItem.NfiVlrIcms      = 0
               endif

          case &CfopCstIcms = '10' // Tributada e c/ cobrança ICMS por S.T.

                // Alteração dia 26/12/2017: (Convenio ICMS 52/2017) segundo Escritório Contábil Cunha – A partir de 01/01/2018
                If &PedCliTp = 'J' and &PedCliIeIse = 'C' and not Null(&CliIes) and &PedConsFinal = 'S' and &PedCliUfCod <> &EmpUf

                    // Valor da Operação = base de calculo integral ICMS + IPI                            
                    &NfiBseClcSt = &NfiBseClcIcmsST               

                    // ICMS ST DIFAL = (Valor da Operação - ICMS Origem) / (1 - Alíquota Interna) * Alíquota Interna - (Valor da Operação * Alíquota Interestadual)
                    &NfiBseClcSt    = &NfiBseClcSt - &NfiVlrIcms  
                    &difAliq        = (100 - (&PerAliqInt + &SdtNfProItem.NfiPerFCP) ) / 100
                    &NfiBseClcSt    = Round(&NfiBseClcSt / &difAliq, 2)
                    &NfiVlrSt       = Round((&NfiBseClcSt * &PerAliqInt) / 100, 2) - &NfiVlrIcms                                                                                                                   

                Else // Cálculo normal para os demais casos sem a alteração do dia 26/12/2017
                
                   //(Base Substituição = base de calculo integral ICms + IPI)* MVA normal
                   &NfiBseClcSt = round((&NfiBseClcIcmsST * &PerSt / 100) + (&NfiBseClcIcmsST), 2)
                    
                   //base de calculo da substituição * 1aliq. interna 
                   &VlrIcmsubTrib1  = round((&NfiBseClcSt * &PerAliqInt)/100, 2)
                
                   &NfiVlrSt = &VlrIcmsubTrib1 - &NfiVlrIcms                              

                EndIf

                &SdtNfProItem.NfiAlqSt = &PerAliqInt

                if &NfiVlrIcms > 0
                    &SdtNfProItem.NfiBseClcIcms   = &NfiBseClcIcms
                    &SdtNfProItem.NfiVlrIcms      = &NfiVlrIcms
                else
                    &SdtNfProItem.NfiBseClcIcms   = 0
                    &SdtNfProItem.NfiVlrIcms      = 0
                endif  

                if &NfiVlrSt > 0
                    &SdtNfProItem.NfiBseClcSt   = &NfiBseClcSt 
                    &SdtNfProItem.NfiVlrSt      = &NfiVlrSt
                else
                    &SdtNfProItem.NfiBseClcSt   = 0
                    &SdtNfProItem.NfiVlrSt      = 0
                endif 

                // Fundo de combate a pobreza da ST
                if &NfiVlrSt > 0 and &SdtNfProItem.NfiPerFCP > 0
                    &SdtNfProItem.NfiVlrBseClcFCPSub    = &NfiBseClcSt
                    &SdtNfProItem.NfiPercFCPSub         = &SdtNfProItem.NfiPerFCP
                    &SdtNfProItem.NfiVlrFCPSub          = round(&NfiBseClcSt * &SdtNfProItem.NfiPerFCP / 100,2)
                endif

          case &CfopCstIcms = '20'  // Com redução de base de calculo

               &SdtNfProItem.NfiBseClcIcmsInt = &NfiBseClcIcmsInt

               &PerRedBC = 100 - &CfopBseIcms

               &SdtNfProItem.NfiPerRedBc =  &PerRedBC

               If &CfopMotDesoneracao = '3' or &CfopMotDesoneracao = '9' or &CfopMotDesoneracao = '12'
                   &NfiIcmsDes = &NfiBseClcIcms * &PerRedBC / 100
                   &NfiIcmsDes = &NfiIcmsDes * &PerIcms / 100
               Else
                   &NfiIcmsDes = 0
               EndIf
            
               //calcula a base de calculo
               &NfiBseClcIcms = &NfiBseClcIcms * &CfopBseIcms / 100
               
               &NfiVlrIcms = &NfiBseClcIcms * &PerIcms / 100               

               if &NfiVlrIcms > 0
                  &SdtNfProItem.NfiBseClcIcms   = &NfiBseClcIcms
                  &SdtNfProItem.NfiVlrIcms      = &NfiVlrIcms
               else
                  &SdtNfProItem.NfiBseClcIcms   = 0
                  &SdtNfProItem.NfiVlrIcms      = 0
               endif

               &SdtNfProItem.NfiIcmsDes = &NfiIcmsDes

          case &CfopCstIcms = '30' // Isenta ou não tributada e c/ cobrança ICMS por S.T

                // Alteração dia 26/12/2017: (Convenio ICMS 52/2017) segundo Escritório Contábil Cunha – A partir de 01/01/2018
                If &PedCliTp = 'J' and &PedCliIeIse = 'C' and not Null(&CliIes) and &PedConsFinal = 'S' and &PedCliUfCod <> &EmpUf

                    // Valor da Operação = base de calculo integral ICMS + IPI                            
                    &NfiBseClcSt = &NfiBseClcIcmsST               

                    // ICMS ST DIFAL = (Valor da Operação - ICMS Origem) / (1 - Alíquota Interna) * Alíquota Interna - (Valor da Operação * Alíquota Interestadual)
                    &NfiBseClcSt    = &NfiBseClcSt - &NfiVlrIcms  
                    &difAliq        = (100 - (&PerAliqInt + &SdtNfProItem.NfiPerFCP) ) / 100
                    &NfiBseClcSt    = Round(&NfiBseClcSt / &difAliq, 2)
                    &NfiVlrSt       = Round((&NfiBseClcSt * &PerAliqInt) / 100, 2) - &NfiVlrIcms                                                                      

                Else // Cálculo normal para os demais casos sem a alteração do dia 26/12/2017
    
                  //(Base Substituição = base de calculo integral ICms + IPI)* MVA normal
                   &NfiBseClcSt = Round((&NfiBseClcIcmsST * &PerSt / 100) + &NfiBseClcIcmsST ,2)
                    
                   //base de calculo da substituição * 1aliq. interna 
                   &VlrIcmsubTrib1  = round((&NfiBseClcSt * &PerAliqInt)/100,2)
                
                   &NfiVlrSt = &VlrIcmsubTrib1 - &NfiVlrIcms        

                EndIf

                &SdtNfProItem.NfiAlqSt = &PerAliqInt
                
                if &NfiVlrSt > 0
                    &SdtNfProItem.NfiBseClcSt   = &NfiBseClcSt 
                    &SdtNfProItem.NfiVlrSt      = &NfiVlrSt
                else
                    &SdtNfProItem.NfiBseClcSt   = 0
                    &SdtNfProItem.NfiVlrSt      = 0
                endif    
                
                &NfiVlrIcms = 0
                &NfiBseClcIcms = 0
                
                &SdtNfProItem.NfiVlrIcms    = 0
                &SdtNfProItem.NfiBseClcIcms = 0

                // Fundo de combate a pobreza da ST
                if &NfiVlrSt > 0 and &SdtNfProItem.NfiPerFCP > 0
                    &SdtNfProItem.NfiVlrBseClcFCPSub    = &NfiBseClcSt
                    &SdtNfProItem.NfiPercFCPSub         = &SdtNfProItem.NfiPerFCP
                    &SdtNfProItem.NfiVlrFCPSub      = round(&NfiBseClcSt * &SdtNfProItem.NfiPerFCP / 100,2)
                endif

          case &CfopCstIcms = '40' or &CfopCstIcms = '41' or &CfopCstIcms = '50' // 40 - Isenta   41 - Não Tributada   50 - Suspensão
            
               //calcula a base de calculo
               &NfiBseClcIcms = &NfiBseClcIcms * &CfopBseIcms / 100
               
               &NfiVlrIcms = &NfiBseClcIcms * &PerIcms / 100

               &NfiIcmsDes = &NfiVlrIcms

               &SdtNfProItem.NfiIcmsDes = &NfiIcmsDes

               &NfiBseClcIcms  = 0
               &NfiVlrIcms = 0

          case &CfopCstIcms = '51' // Diferimento

               &SdtNfProItem.NfiBseClcIcmsInt = &NfiBseClcIcmsInt

               if &CfopBseIcms > 0 and &CfopBseIcms < 100 // Tem redução da base do ICMS
                    &PerRedBC = 100 - &CfopBseIcms
                    &SdtNfProItem.NfiPerRedBc = &PerRedBC
               Else // Não tem redução da base do ICMS
                    &PerRedBC = 100 // Valor da base do ICMS será 100%, não irá reduzir a base
                    &SdtNfProItem.NfiPerRedBc = 0 // Não haverá % de redução da base do ICMS
               EndIf               
           
               &SdtNfProItem.NfiBseClcIcms = Round((&NfiBseClcIcms * &PerRedBC / 100),2) // Valor da BC do ICMS            
               &SdtNfProItem.NfiVlrIcmsDiferimento = Round((&SdtNfProItem.NfiBseClcIcms * &PerIcms / 100),2) // Valor do ICMS da Operação                   
               &SdtNfProItem.NfiPerDiferimento = &CfopPerDiferimento // % do Diferimento
               &SdtNfProItem.NfiVlrDiferimento = Round((&SdtNfProItem.NfiVlrIcmsDiferimento * &CfopPerDiferimento / 100),2) // Valor do ICMS Diferido
               &SdtNfProItem.NfiVlrIcms = &SdtNfProItem.NfiVlrIcmsDiferimento - &SdtNfProItem.NfiVlrDiferimento // Valor do ICMS 

               if &NfiVlrIcms <= 0
                  &SdtNfProItem.NfiBseClcIcms   = 0
                  &SdtNfProItem.NfiVlrIcms      = 0
               endif

               &SdtNfProItem.NfiVlrSt      = 0
               &SdtNfProItem.NfiBseClcSt   = 0

          case &CfopCstIcms = '60' // 60 - ICMS cobrado anteriormente por S.T.

                Do 'UltNfCompra' // Busca os dados do produto na última nota de compra

                &NfiAliqSt060 = &NicAlqIcmsSt // Alíquota suportada pelo ConsumidorFinal

                If &NicQtd > 0
                    &NicBseClcStUnit = Round( &NicBseClcSt / &NicQtd ,8) // Base Calculo ST Unitário
                    &NicVlrStUnit = Round( &NicVlrSt / &NicQtd ,8) // Valor Unitário do ICMS ST
                    &NicVlrIcmsUnit = Round( &NicVlrIcms / &NicQtd ,8) // Valor Unitário do ICMS 
                EndIf

                if &CfopBseIcms > 0 and &CfopBseIcms < 100 // Tem redução da base do ICMS
                    &PerRedBC = 100 - &CfopBseIcms
                    &SdtNfProItem.NfiPerRedBc = &PerRedBC
                Else // Não tem redução da base do ICMS
                    &PerRedBC = 100 // Valor da base do ICMS será 100%, não irá reduzir a base
                    &SdtNfProItem.NfiPerRedBc = 0 // Não haverá % de redução da base do ICMS
                EndIf  

                &NfiBseClcSt060 = Round( &NicBseClcStUnit * &PdiQtd ,2) // Valor da Base ICMS Retido
                &NfiVlrSt060 = Round( &NicVlrStUnit * &PdiQtd ,2) // Valor do ICMS Retido 

                &NfiVlrBseClcIcmsEfet = Round( (&PdiVlrUnt * &PerRedBC) / 100 * &PdiQtd ,2) // Valor da base de cálculo efetiva
                &NfiPercIcmsEfet = &NicAlqIcmsSt // Alíquota do ICMS efetiva
                &NfiVlrIcmsEfet = Round( &NfiVlrBseClcIcmsEfet * &NfiPercIcmsEfet / 100 ,2) // Valor do ICMS efetivo

                &NfiVlrIcmsSubstituto = Round( &NicVlrIcmsUnit * &PdiQtd ,2) // Valor do ICMS próprio do Substituto                

                &NfiParcelaIcmsRetido = &NfiVlrIcmsEfet - &NfiVlrIcmsSubstituto - &NfiVlrSt060 // Parcela do ICMS retido                
                
                &SdtNfProItem.NfiBseClcSt060 = &NfiBseClcSt060 
                &SdtNfProItem.NfiVlrSt060 = &NfiVlrSt060
                &SdtNfProItem.NfiAliqSt060 = &NfiAliqSt060
                &SdtNfProItem.NfiVlrBseClcIcmsEfet = &NfiVlrBseClcIcmsEfet
                &SdtNfProItem.NfiPercIcmsEfet = &NfiPercIcmsEfet
                &SdtNfProItem.NfiVlrIcmsEfet = &NfiVlrIcmsEfet
                &SdtNfProItem.NfiVlrIcmsSubstituto = &NfiVlrIcmsSubstituto
                &SdtNfProItem.NfiParcelaIcmsRetido = &NfiParcelaIcmsRetido
                
                &SdtNfProItem.NfiVlrIcms    = 0
                &SdtNfProItem.NfiBseClcIcms = 0
                &SdtNfProItem.NfiVlrSt      = 0
                &SdtNfProItem.NfiBseClcSt   = 0

                &NfiVlrIcms      = 0
                &NfiBseClcSt     = 0
                &NfiBseClcSt060  = 0
                &NfiVlrSt060     = 0
                &NicBseClcStUnit = 0
                &NicVlrStUnit = 0
                &NicVlrIcmsUnit = 0 
                &NfiVlrBseClcIcmsEfet = 0
                &NfiPercIcmsEfet = 0
                &NfiVlrIcmsEfet = 0
                &NfiVlrIcmsSubstituto = 0                
                &NfiParcelaIcmsRetido = 0

          case &CfopCstIcms = '70' // 70 - Com redução de B.C. e cobrança ICMS por S.T.
            
               &PerRedBC = 100 - &CfopBseIcms

               &SdtNfProItem.NfiBseClcIcmsInt = &NfiBseClcIcmsInt
               &SdtNfProItem.NfiPerRedBc = &PerRedBC

               If &CfopMotDesoneracao = '3' or &CfopMotDesoneracao = '9' or &CfopMotDesoneracao = '12'
                   &NfiIcmsDes = &NfiBseClcIcms * &PerRedBC / 100
                   &NfiIcmsDes = &NfiIcmsDes * &PerIcms / 100
               Else
                   &NfiIcmsDes = 0
               EndIf

               //calcula a base de calculo
               &NfiBseClcIcms = Round(&NfiBseClcIcms * &CfopBseIcms / 100, 2)
               
               &NfiVlrIcms = Round(&NfiBseClcIcms * &PerIcms / 100, 2)

               &NfiBseClcIcmsST = Round((&NfiBseClcIcmsST * &CfopBseIcmsSt) / 100, 2)

                // Alteração dia 26/12/2017: (Convenio ICMS 52/2017) segundo Escritório Contábil Cunha – A partir de 01/01/2018
                If &PedCliTp = 'J' and &PedCliIeIse = 'C' and not Null(&CliIes) and &PedConsFinal = 'S' and &PedCliUfCod <> &EmpUf                    

                    // Valor da Operação = base de calculo integral ICMS + IPI                            
                    &NfiBseClcSt = &NfiBseClcIcmsST               

                    // ICMS ST DIFAL = (Valor da Operação - ICMS Origem) / (1 - Alíquota Interna) * Alíquota Interna - (Valor da Operação * Alíquota Interestadual)
                    &NfiBseClcSt    = &NfiBseClcSt - &NfiVlrIcms  
                    &difAliq        = (100 - (&PerAliqInt + &SdtNfProItem.NfiPerFCP) ) / 100
                    &NfiBseClcSt    = Round(&NfiBseClcSt / &difAliq, 2)
                    &NfiVlrSt       = Round((&NfiBseClcSt * &PerAliqInt) / 100, 2) - &NfiVlrIcms                                                                      

                Else // Cálculo normal para os demais casos sem a alteração do dia 26/12/2017
                    
                    //(Base Substituição = base de calculo integral ICms + IPI)* MVA normal (aqui usa-se a base do ICMS ST com redução que já está com IPI) 
                    &NfiBseClcSt = Round((&NfiBseClcIcmsST * &PerSt / 100) + &NfiBseClcIcmsST, 2)
                    
                    //base de calculo da substituição * 1aliq. interna 
                    &VlrIcmsubTrib1  = round((&NfiBseClcSt * &PerAliqInt)/100, 2)                    
                    
                    &NfiVlrSt = &VlrIcmsubTrib1 - &NfiVlrIcms                                       

                EndIf

                &SdtNfProItem.NfiAlqSt = &PerAliqInt

                if &NfiVlrIcms > 0
                    &SdtNfProItem.NfiBseClcIcms = &NfiBseClcIcms
                    &SdtNfProItem.NfiVlrIcms    = &NfiVlrIcms
                else
                    &SdtNfProItem.NfiBseClcIcms = 0
                    &SdtNfProItem.NfiVlrIcms    = 0
                endif  

                if &NfiVlrSt > 0
                    &SdtNfProItem.NfiBseClcSt   = &NfiBseClcSt 
                    &SdtNfProItem.NfiVlrSt      = &NfiVlrSt
                else
                    &SdtNfProItem.NfiBseClcSt   = 0
                    &SdtNfProItem.NfiVlrSt      = 0
                endif   
             
                &SdtNfProItem.NfiIcmsDes = &NfiIcmsDes

                // Fundo de combate a pobreza da ST
                if &NfiVlrSt > 0 and &SdtNfProItem.NfiPerFCP > 0
                    &SdtNfProItem.NfiVlrBseClcFCPSub    = &NfiBseClcSt
                    &SdtNfProItem.NfiPercFCPSub         = &SdtNfProItem.NfiPerFCP
                    &SdtNfProItem.NfiVlrFCPSub      = round(&NfiBseClcSt * &SdtNfProItem.NfiPerFCP / 100,2)
                endif

          case &CfopCstIcms = '90' // Outras

               &PerRedBC = 100 - &CfopBseIcms

               If &CfopMotDesoneracao = '3' or &CfopMotDesoneracao = '9' or &CfopMotDesoneracao = '12'
                   &NfiIcmsDes = &NfiBseClcIcms * &PerRedBC / 100
                   &NfiIcmsDes = &NfiIcmsDes * &PerIcms / 100
               Else
                   &NfiIcmsDes = 0
               EndIf
            
               //calcula a base de calculo
               &NfiBseClcIcms = Round(&NfiBseClcIcms * &CfopBseIcms / 100, 2)
               
               &NfiVlrIcms = Round(&NfiBseClcIcms * &PerIcms / 100, 2)

               &NfiBseClcIcmsST = Round((&NfiBseClcIcmsST * &CfopBseIcmsSt) / 100, 2)

                // Alteração dia 26/12/2017: (Convenio ICMS 52/2017) segundo Escritório Contábil Cunha – A partir de 01/01/2018
                If &PedCliTp = 'J' and &PedCliIeIse = 'C' and not Null(&CliIes) and &PedConsFinal = 'S' and &PedCliUfCod <> &EmpUf                    

                    // Valor da Operação = base de calculo integral ICMS + IPI                            
                    &NfiBseClcSt = &NfiBseClcIcmsST               

                    // ICMS ST DIFAL = (Valor da Operação - ICMS Origem) / (1 - Alíquota Interna) * Alíquota Interna - (Valor da Operação * Alíquota Interestadual)
                    &NfiBseClcSt    = &NfiBseClcSt - &NfiVlrIcms  
                    &difAliq        = (100 - (&PerAliqInt + &SdtNfProItem.NfiPerFCP) ) / 100
                    &NfiBseClcSt    = Round(&NfiBseClcSt / &difAliq, 2)
                    &NfiVlrSt       = Round((&NfiBseClcSt * &PerAliqInt) / 100, 2) - &NfiVlrIcms                                                                    

                Else // Cálculo normal para os demais casos sem a alteração do dia 26/12/2017
    
                   //(Base Substituição = base de calculo integral ICms + IPI)* MVA normal (aqui usa-se a base do ICMS ST com redução que já está com IPI) 
                   &NfiBseClcSt = Round((&NfiBseClcIcmsST * &PerSt / 100) + &NfiBseClcIcmsST, 2)
                    
                   //base de calculo da substituição * 1aliq. interna 
                   &VlrIcmsubTrib1  = round((&NfiBseClcSt * &PerAliqInt)/100, 2)
                
                   &NfiVlrSt = &VlrIcmsubTrib1 - &NfiVlrIcms
                          
                EndIf
                
                &SdtNfProItem.NfiAlqSt = &PerAliqInt                                        
                
                if &NfiVlrIcms > 0
                    &SdtNfProItem.NfiBseClcIcms = &NfiBseClcIcms
                    &SdtNfProItem.NfiVlrIcms    = &NfiVlrIcms
                else
                    &SdtNfProItem.NfiBseClcIcms = 0
                    &SdtNfProItem.NfiVlrIcms    = 0
                endif  
                
                if &NfiVlrSt > 0
                    &SdtNfProItem.NfiBseClcSt   = &NfiBseClcSt 
                    &SdtNfProItem.NfiVlrSt      = &NfiVlrSt
                else
                    &SdtNfProItem.NfiBseClcSt   = 0
                    &SdtNfProItem.NfiVlrSt      = 0
                endif   
                
                &SdtNfProItem.NfiIcmsDes = &NfiIcmsDes

                // Fundo de combate a pobreza da ST
                if &NfiVlrSt > 0 and &SdtNfProItem.NfiPerFCP > 0
                    &SdtNfProItem.NfiVlrBseClcFCPSub    = &NfiBseClcSt
                    &SdtNfProItem.NfiPercFCPSub         = &SdtNfProItem.NfiPerFCP
                    &SdtNfProItem.NfiVlrFCPSub      = round(&NfiBseClcSt * &SdtNfProItem.NfiPerFCP / 100,2)
                endif
       endcase

    else
    
        do case
           case &CfopCsosn = '101' // 101 - Tributada pelo Simples Nacional com permissão de crédito
           
                if &PedTpDsc = 2 and &EmpDescIncICMS = 1 // Alteração dia 24/11/17: Desconto incondicional desconta na base de cálculo do ICMS (se estiver marcado a opção nos parâmetros da empresa)
                    &vlritm = round(&PdiTotVlr + &PedFrete + &PedAcr - &PedDsc,2)
                else
                    &vlritm = round(&PdiTotVlr + &PedFrete + &PedAcr,2)
                endif
        
                &SdtNfProItem.NfiVlrCredIcms = Round((&vlritm * &EmpPerCredIcms) / 100, 2)
    
           case &CfopCsosn = '102' or &CfopCsosn = '103' or &CfopCsosn = '300' or &CfopCsosn = '400'
    
                &SdtNfProItem.NfiVlrIcms    = 0
                &SdtNfProItem.NfiBseClcIcms = 0
                &SdtNfProItem.NfiVlrSt      = 0
                &SdtNfProItem.NfiBseClcSt   = 0
    
          case &CfopCsosn = '201' // Tributada pelo Simples Nacional com permissão de crédito e com cobrança do ICMS por ST   

            // Alteração dia 26/12/2017: (Convenio ICMS 52/2017) segundo Escritório Contábil Cunha – A partir de 01/01/2018
            If &PedCliTp = 'J' and &PedCliIeIse = 'C' and not Null(&CliIes) and &PedConsFinal = 'S' and &PedCliUfCod <> &EmpUf

                // Valor da Operação = base de calculo integral ICMS + IPI                            
                &NfiBseClcSt = &NfiBseClcIcmsST               

                // ICMS ST DIFAL = (Valor da Operação - ICMS Origem) / (1 - Alíquota Interna) * Alíquota Interna - (Valor da Operação * Alíquota Interestadual)
                &NfiBseClcSt    = &NfiBseClcSt - &NfiVlrIcms  
                &difAliq        = (100 - (&PerAliqInt + &SdtNfProItem.NfiPerFCP) ) / 100
                &NfiBseClcSt    = Round(&NfiBseClcSt / &difAliq, 2)
                &NfiVlrSt       = Round((&NfiBseClcSt * &PerAliqInt) / 100, 2) - &NfiVlrIcms                                                                      

            Else // Cálculo normal para os demais casos sem a alteração do dia 26/12/2017
        
                ///(Base Substituição = base de calculo integral ICms + IPI)* MVA normal
                &NfiBseClcSt = Round((&NfiBseClcIcmsST * &PerSt / 100) + &NfiBseClcIcmsST, 2)
                        
                //base de calculo da substituição * 1aliq. interna 
                &VlrIcmsubTrib1  = round((&NfiBseClcSt * &PerAliqInt)/100,2)
                    
                &NfiVlrSt = &VlrIcmsubTrib1 - &NfiVlrIcms        

            EndIf

            &SdtNfProItem.NfiAlqSt = &PerAliqInt

            if &NfiVlrSt > 0
                &SdtNfProItem.NfiBseClcSt   = &NfiBseClcSt 
                &SdtNfProItem.NfiVlrSt      = &NfiVlrSt
            else
                &SdtNfProItem.NfiBseClcSt   = 0
                &SdtNfProItem.NfiVlrSt      = 0
            endif 
            
            &SdtNfProItem.NfiBseClcIcms     = 0
            &SdtNfProItem.NfiVlrIcms        = 0 

            if &PedTpDsc = 2 and &EmpDescIncICMS = 1 // Alteração dia 24/11/17: Desconto incondicional desconta na base de cálculo do ICMS (se estiver marcado a opção nos parâmetros da empresa)
                &vlritm = round(&PdiTotVlr + &PedFrete + &PedAcr - &PedDsc,2)
            else
                &vlritm = round(&PdiTotVlr + &PedFrete + &PedAcr,2)
            endif
    
            &SdtNfProItem.NfiVlrCredIcms = (&vlritm * &EmpPerCredIcms) / 100

            // Fundo de combate a pobreza da ST
            if &NfiVlrSt > 0 and &SdtNfProItem.NfiPerFCP > 0
                &SdtNfProItem.NfiVlrBseClcFCPSub    = &NfiBseClcSt
                &SdtNfProItem.NfiPercFCPSub         = &SdtNfProItem.NfiPerFCP
                &SdtNfProItem.NfiVlrFCPSub      = round(&NfiBseClcSt * &SdtNfProItem.NfiPerFCP / 100,2)
            endif
        
         case &CfopCsosn = '202' or &CfopCsosn = '203' // 202 - Tributada pelo Simples Nacional sem permissão de crédito e com cobrança do ICMS por ST      203 - Isenção do ICMS no Simples Nacional para faixa de receita bruta e com cobrança do ICMS ST

            
            //(Base Substituição = base de calculo integral ICms + IPI)* MVA normal
            &NfiBseClcSt = ((&NfiBseClcIcmsST) * &PerSt / 100) + (&NfiBseClcIcmsST)
                    
            //base de calculo da substituição * 1aliq. interna 
            &VlrIcmsubTrib1  = round((&NfiBseClcSt * &PerAliqInt)/100, 2)
                
            &NfiVlrSt = &VlrIcmsubTrib1 - &NfiVlrIcms                                

            &SdtNfProItem.NfiAlqSt = &PerAliqInt

            if &NfiVlrSt > 0
                &SdtNfProItem.NfiBseClcSt   = &NfiBseClcSt 
                &SdtNfProItem.NfiVlrSt      = &NfiVlrSt
            else
                &SdtNfProItem.NfiBseClcSt   = 0
                &SdtNfProItem.NfiVlrSt      = 0
            endif 
            
            &SdtNfProItem.NfiBseClcIcms = 0
            &SdtNfProItem.NfiVlrIcms    = 0 
 
            // Fundo de combate a pobreza da ST
            if &NfiVlrSt > 0 and &SdtNfProItem.NfiPerFCP > 0
                &SdtNfProItem.NfiVlrBseClcFCPSub    = &NfiBseClcSt
                &SdtNfProItem.NfiPercFCPSub         = &SdtNfProItem.NfiPerFCP
                &SdtNfProItem.NfiVlrFCPSub      = round(&NfiBseClcSt * &SdtNfProItem.NfiPerFCP / 100,2)
            endif
    
          case &CfopCsosn = '500' // ICMS cobrado anteriormente por ST ou por antecipação


                Do 'UltNfCompra' // Busca os dados do produto na última nota de compra

                &NfiAliqSt060 = &NicAlqIcmsSt // Alíquota suportada pelo ConsumidorFinal

                If &NicQtd > 0
                    &NicBseClcStUnit = Round( &NicBseClcSt / &NicQtd ,8) // Base Calculo ST Unitário
                    &NicVlrStUnit = Round( &NicVlrSt / &NicQtd ,8) // Valor Unitário do ICMS ST
                    &NicVlrIcmsUnit = Round( &NicVlrIcms / &NicQtd ,8) // Valor Unitário do ICMS 
                EndIf

                if &CfopBseIcms > 0 and &CfopBseIcms < 100 // Tem redução da base do ICMS
                    &PerRedBC = 100 - &CfopBseIcms
                    &SdtNfProItem.NfiPerRedBc = &PerRedBC
                Else // Não tem redução da base do ICMS
                    &PerRedBC = 100 // Valor da base do ICMS será 100%, não irá reduzir a base
                    &SdtNfProItem.NfiPerRedBc = 0 // Não haverá % de redução da base do ICMS
                EndIf  

                &NfiBseClcSt060 = Round( &NicBseClcStUnit * &PdiQtd ,2) // Valor da Base ICMS Retido
                &NfiVlrSt060 = Round( &NicVlrStUnit * &PdiQtd ,2) // Valor do ICMS Retido 

                &NfiVlrBseClcIcmsEfet = Round( (&PdiVlrUnt * &PerRedBC) / 100 * &PdiQtd ,2) // Valor da base de cálculo efetiva
                &NfiPercIcmsEfet = &NicAlqIcmsSt // Alíquota do ICMS efetiva
                &NfiVlrIcmsEfet = Round( &NfiVlrBseClcIcmsEfet * &NfiPercIcmsEfet / 100 ,2) // Valor do ICMS efetivo

                &NfiVlrIcmsSubstituto = Round( &NicVlrIcmsUnit * &PdiQtd ,2) // Valor do ICMS próprio do Substituto                

                &NfiParcelaIcmsRetido = &NfiVlrIcmsEfet - &NfiVlrIcmsSubstituto - &NfiVlrSt060 // Parcela do ICMS retido                
                
                &SdtNfProItem.NfiBseClcSt060 = &NfiBseClcSt060 
                &SdtNfProItem.NfiVlrSt060 = &NfiVlrSt060
                &SdtNfProItem.NfiAliqSt060 = &NfiAliqSt060
                &SdtNfProItem.NfiVlrBseClcIcmsEfet = &NfiVlrBseClcIcmsEfet
                &SdtNfProItem.NfiPercIcmsEfet = &NfiPercIcmsEfet
                &SdtNfProItem.NfiVlrIcmsEfet = &NfiVlrIcmsEfet
                &SdtNfProItem.NfiVlrIcmsSubstituto = &NfiVlrIcmsSubstituto
                &SdtNfProItem.NfiParcelaIcmsRetido = &NfiParcelaIcmsRetido
                
                &SdtNfProItem.NfiVlrIcms    = 0
                &SdtNfProItem.NfiBseClcIcms = 0
                &SdtNfProItem.NfiVlrSt      = 0
                &SdtNfProItem.NfiBseClcSt   = 0

                &NfiVlrIcms      = 0
                &NfiBseClcSt     = 0
                &NfiBseClcSt060  = 0
                &NfiVlrSt060     = 0
                &NicBseClcStUnit = 0
                &NicVlrStUnit = 0
                &NicVlrIcmsUnit = 0 
                &NfiVlrBseClcIcmsEfet = 0
                &NfiPercIcmsEfet = 0
                &NfiVlrIcmsEfet = 0
                &NfiVlrIcmsSubstituto = 0                
                &NfiParcelaIcmsRetido = 0

    
         case &CfopCsosn = '900' 

               &PerRedBC = 100 - &CfopBseIcms
            
               //calcula a base de calculo
               &NfiBseClcIcms = Round(&NfiBseClcIcms * &CfopBseIcms / 100, 2)
               
               &NfiVlrIcms = Round(&NfiBseClcIcms * &PerIcms / 100, 2)

               &NfiBseClcIcmsST = Round((&NfiBseClcIcmsST * &CfopBseIcmsSt) / 100, 2)

                // Alteração dia 26/12/2017: (Convenio ICMS 52/2017) segundo Escritório Contábil Cunha – A partir de 01/01/2018
                If &PedCliTp = 'J' and &PedCliIeIse = 'C' and not Null(&CliIes) and &PedConsFinal = 'S' and &PedCliUfCod <> &EmpUf                    

                    // Valor da Operação = base de calculo integral ICMS + IPI                            
                    &NfiBseClcSt = &NfiBseClcIcmsST               

                    // ICMS ST DIFAL = (Valor da Operação - ICMS Origem) / (1 - Alíquota Interna) * Alíquota Interna - (Valor da Operação * Alíquota Interestadual)
                    &NfiBseClcSt    = &NfiBseClcSt - &NfiVlrIcms  
                    &difAliq        = (100 - (&PerAliqInt + &SdtNfProItem.NfiPerFCP) ) / 100
                    &NfiBseClcSt    = Round(&NfiBseClcSt / &difAliq, 2)
                    &NfiVlrSt       = Round((&NfiBseClcSt * &PerAliqInt) / 100, 2) - &NfiVlrIcms                                                                      

                Else // Cálculo normal para os demais casos sem a alteração do dia 26/12/2017
    
                   //(Base Substituição = base de calculo integral ICms + IPI)* MVA normal (aqui usa-se a base do ICMS ST com redução que já está com IPI) 
                   &NfiBseClcSt = Round((&NfiBseClcIcmsST * &PerSt / 100) + &NfiBseClcIcmsST, 2)
                    
                   //base de calculo da substituição * 1aliq. interna 
                   &VlrIcmsubTrib1  = round((&NfiBseClcSt * &PerAliqInt)/100, 2)
                
                   &NfiVlrSt = &VlrIcmsubTrib1 - &NfiVlrIcms
                          
                EndIf
                
                &SdtNfProItem.NfiAlqSt = &PerAliqInt                                        
                
                if &NfiVlrIcms > 0
                    &SdtNfProItem.NfiBseClcIcms = &NfiBseClcIcms
                    &SdtNfProItem.NfiVlrIcms    = &NfiVlrIcms
                else
                    &SdtNfProItem.NfiBseClcIcms = 0
                    &SdtNfProItem.NfiVlrIcms    = 0
                endif  
                
                if &NfiVlrSt > 0
                    &SdtNfProItem.NfiBseClcSt   = &NfiBseClcSt 
                    &SdtNfProItem.NfiVlrSt      = &NfiVlrSt
                else
                    &SdtNfProItem.NfiBseClcSt   = 0
                    &SdtNfProItem.NfiVlrSt      = 0
                endif                   

                // Fundo de combate a pobreza da ST
                if &NfiVlrSt > 0 and &SdtNfProItem.NfiPerFCP > 0
                    &SdtNfProItem.NfiVlrBseClcFCPSub    = &NfiBseClcSt
                    &SdtNfProItem.NfiPercFCPSub         = &SdtNfProItem.NfiPerFCP
                    &SdtNfProItem.NfiVlrFCPSub      = round(&NfiBseClcSt * &SdtNfProItem.NfiPerFCP / 100,2)
                endif
        
                if &PedTpDsc = 2 and &EmpDescIncICMS = 1 // Alteração dia 24/11/17: Desconto incondicional desconta na base de cálculo do ICMS (se estiver marcado a opção nos parâmetros da empresa)
                    &vlritm = round(&PdiTotVlr + &PedFrete + &PedAcr - &PedDsc,2)
                else
                    &vlritm = round(&PdiTotVlr + &PedFrete + &PedAcr,2)
                endif
    
                &SdtNfProItem.NfiVlrCredIcms = (&vlritm * &EmpPerCredIcms) / 100                    
    
       endcase
    endif
endsub


sub'zonafranca'
    &CliInsSuf = ''
    for each CliCod
       where CliCod = &PedCliCod
    
       &CliInsSuf = CliInsSuf
       &CliUfCod = CliUfCod
       &CliCidCod = CliCidCod
       &CliSuframaCOFINS = CliSuframaCOFINS
       &CliSuframaICMS = CliSuframaICMS
       &CliSuframaPIS = CliSuframaPIS
       &CliCnpjEndEnt = CliCnpjEndEnt
       &CliCpfEndEnt  = CliCpfEndEnt
       &CliEndEnt = CliEndEnt
       &CliEndNumEnt = CliEndNumEnt
       &CliEndComplEnt = CliEndComplEnt
       &CliEndBaiEnt = CliEndBaiEnt
       &CliCidNomEnt = CliCidNomEnt
       &CliUfCodEnt = CliUfCodEnt
       &CliIes = CliIes
       &CliInfCmp = CliInfCmp
    endfor
    
    if not null(&CliInsSuf)
       for each CidCod
          where CidCod = &CliCidCod
    
           If &CliSuframaICMS = 'S'
                &CidDesIcms = CidDesIcms
           EndIf
           If &CliSuframaPIS = 'S'
                &CidDesPis  = CidDesPis 
           EndIf
           If &CliSuframaCOFINS = 'S'
                &CidDesCof  = CidDesCof
           EndIf
    
        endfor
    else
       &CidDesIcms = 0
       &CidDesPis  = 0
       &CidDesCof  = 0
    endif
endsub


sub'partilha'

    For Each UfCod
        Where UfCod = &PedCliUfCod
            &UFCalcPartilha = UFCalcPartilha // Verifica no cadastro de estado o tipo de cálculo da Partilha do ICMS
    EndFor

    do case
        case year(&Today) = 2015 or year(&Today) = 15
             &percentual = 40
        case year(&Today) = 2016 or year(&Today) = 16 
             &percentual = 40
        case year(&Today) = 2017 or year(&Today) = 17
             &percentual = 60
        case year(&Today) = 2018 or year(&Today) = 18
             &percentual = 80
        otherwise
             &percentual = 100
    endcase  
    
    //acha a base de calculo do icms de origem
    &BCInterestadual = &PdiTotVlr
    &BCInterestadual += round(&PedFrete,2)
    &BCInterestadual += round(&PedAcr,2)
    &BCInterestadual += &NfiVlrIpi 
    
    if &PedTpDsc = 2 and &EmpDescIncICMS = 1 // Alteração dia 24/11/17: Desconto incondicional desconta na base de cálculo do ICMS (se estiver marcado a opção nos parâmetros da empresa)
       &BCInterestadual -= round(&PedDsc,2)
    endif  

    &BCInterestadualSemReducao = &BCInterestadual

//    Msg(('Antes'))
//    Msg(ToFormattedString(&BCInterestadual))


    if &CfopBseIcms > 0 and &CfopBseIcms < 100
        &BCInterestadual = &BCInterestadual - ( &BCInterestadual * ((100 - &CfopBseIcms) / 100))      //ALterado para calcular com redução 
    endif   
//     Msg(('Depois'))
//    Msg(ToFormattedString(&BCInterestadual))

        
    &SdtNfProItem.NfiVlrBseIcmsDest = &BCInterestadual  //  Valor da BC do ICMS na UF de destino
    &NfiVlrIcms = &BCInterestadual * &PerIcms / 100     //  Valor do ICMS
    &SdtNfProItem.NfiAlqInter = &PerAliqInt             //  Aliquota do ICMS Interna     
    &SdtNfProItem.NfiAlqInterestadual = &PerIcms        //  Aliquota do ICMS Interestadual 
    
    If &percentual > 0 

        do case 
           case &UfCalcPartilha = 'S'  // Base de calculo do ICMS por dentro 

                &difAliq           = round( ( (100 - (&PerAliqInt + &SdtNfProItem.NfiPerFCP) ) / 100),2) // Este percentual interno está pegando do cadastro de MVA 
                &BaseDIFAL         = round( ( (&BCInterestadual - &NfiVlrIcms) / &difAliq),2)

//     Msg(('&difAliq'))
//    Msg(ToFormattedString(&difAliq))
//
//     Msg(('&BaseDIFAL'))
//    Msg(ToFormattedString(&BaseDIFAL))
                                                                                                  
                &SdtNfProItem.NfiVlrFCP = round( ( (&BaseDIFAL * &SdtNfProItem.NfiPerFCP)/100),2)    //VALOR DO FUNDO DE POBREZA
                
                &IcmsInterestadual = round(((round(&BaseDIFAL * &PerIcms,2) ) / 100),2)                                 
                &IcmsInterno       = (&BaseDIFAL * &PerAliqInt) / 100
                                                 
                &SdtNfProItem.NfiVlrBseIcmsDest = &BaseDIFAL // Valor da BC FCP na UF de destino
                        
                &Diferenca         =  round( (&IcmsInterno - &IcmsInterestadual),2)
              
           case &UfCalcPartilha = 'I' or Null(&UfCalcPartilha) // Base de calculo do ICMS 

                &SdtNfProItem.NfiVlrFCP = round( ( (&BCInterestadual * &SdtNfProItem.NfiPerFCP) /100),2) //  VALOR DO FUNDO DE POBREZA            
                &SdtNfProItem.NfiVlrBseIcmsDest = &BCInterestadual //  Base de calculo do ICMS 
                                                                       
                //VALOR ICMS COM ALIQ INTERNA                                                                                     
                &IcmsInterno       = round( ( (&BCInterestadual * &PerAliqInt) /100),2) 
                &Diferenca         = round( (&IcmsInterno - &NfiVlrIcms),2)   
                         
           case &UfCalcPartilha = 'D' // Base de calculo do ICMS por dentro com desconto do ICMS
               
                &DifAliq     = round( ( (100 - (&PerAliqInt + &SdtNfProItem.NfiPerFCP) ) / 100),2)                       // (100 - (Percentual da Aliquota Interna + Percentual de Pobreza) ) / 100 ==> (100 - (18 + 0) ) / 100 = 0,82
                &BaseDIFAL   = round( ( (&BCInterestadual - &NfiVlrIcms) / &DifAliq),2)                 // (Base de Calculo do ICMS - ICMS) /  Diferença aliquota da DIFAL) ==>  ( (2467,42 - 296,09) / 0,82 ) = 2.647,96         
                &IcmsInterno = round( (&BaseDIFAL * ( &PerAliqInt / 100) ),2)                                        //  Base de calculo da DIFAL *  ( (Aliquota ICMS interno  /100) ==>  2.647,96 * 18% = 476,63
                &Diferenca   = round( (&IcmsInterno - &NfiVlrIcms),2)                                          // ICMS Interno - ICMS Interestadual  ==> 476.63 - 296.09 = 180.54
                                                       
                &SdtNfProItem.NfiVlrFCP = round( (&BaseDIFAL * (&SdtNfProItem.NfiPerFCP / 100)),2)                 // Valor do ICMS relativo ao Fundo de Combate a Pobreza  ==> 2647.96 * 0% = 0
                &SdtNfProItem.NfiVlrBseIcmsDest = &BaseDIFAL                                                           // Base de calculo do ICMS 
                           
           Case &UFCalcPartilha = 'G'   // Base de Cálculo do ICMS pela diferença das Alíquotas de ICMS Especifico ate a presente data (09/05/2018) para o Estado de Goias Obs: Não existe FCP
                //&PerIcms                                                                    //1-DIFAL = Alíquota do ICMS  da operação Interestadual) = 7%;
                //&PerAliqInt                                                                 //2-DIFAL = Alíquota do ICMS  Operaç.Interna GO = 17%;
                &DifAliq           = round(((100 - (&PerAliqInt - &PerIcms) ) / 100 ),2)      //3-DIFAL = (Base  do ICMS) = 18% - 7%=10% ,refere-se ao  DIFERENCIAL da alíquota do ICMS.GOIÁS determina que seja incluído o diferencial do ICMS de 10%, neste caso, direto, na BC do ICMS para o cálculo do DIFAL, e considera uma base única para efetuar o cálculo de 7% e de 17%, conforme segue.Considerando o exemplo acima, temos:DIFAL=coeficiente para ICMS por dentro=100-10=0,90;
                &BaseDIFAL         = round((&BCInterestadual / &DifAliq),2)                   //4-DIFAL = (BASE do ICMS) = 1.100,00/0,90=(BASE ÚNICA  do ICMS- DIFAL)=1.222,22;
                &IcmsInterno       = round(((round(&BaseDIFAL * &PerAliqInt,2) ) / 100),2)    //5-DIFAL = (BASE ÚNICA  do ICMS-DIFAL )=1.222,22 * 17% (Alíquota do ICMS Operaç. Interna destino)=207,78;
                &IcmsInterestadual = round(((round(&BaseDIFAL * &PerIcms,2) ) / 100),2)       //6-DIFAL = (BASE ÚNICA  do ICMS-DIFAL )=1.222,22 * 7% (Alíquota do ICMS Operaç. Interestadual)=85,56;
                &Diferenca         = round(&IcmsInterno - &IcmsInterestadual,2)               //7-DIFAL = CÁLCULO=207,78 – 85,56=DIFAL=122,22;
                                                                                              //8-DIFAL = PARTILHA =122,22 = DESTINO -GO =80% =97,78/ORIGEM-SP=20%=24,44;                   
                &SdtNfProItem.NfiVlrFCP  = round( (&BaseDIFAL * (&SdtNfProItem.NfiPerFCP / 100)),2)  // Valor do ICMS relativo ao Fundo de Combate a Pobreza      
                &SdtNfProItem.NfiVlrBseIcmsDest  = &BaseDIFAL // Valor da BC FCP na UF de destino



           Case &UFCalcPartilha = '9'   // CALCULO DA NOTA TECNICA - NT 05/2020

                

                &difAliq           = round( ( (100 - (&PerAliqInt) ) / 100),2)

                &BaseDIFAL         = round( ( (&BCInterestadualSemReducao) / &difAliq),2)       
                                                                                                  
                &SdtNfProItem.NfiVlrFCP = round( ( (&BaseDIFAL * &SdtNfProItem.NfiPerFCP)/100),2)    //VALOR DO FUNDO DE POBREZA
                
                &IcmsInterestadual = round(((round(&BaseDIFAL * &PerIcms,2) ) / 100),2)                                 
                &IcmsInterno       = (&BaseDIFAL * &PerAliqInt) / 100
                                                 
                &SdtNfProItem.NfiVlrBseIcmsDest = &BaseDIFAL // Valor da BC FCP na UF de destino
                        
                &Diferenca         =  round( (&IcmsInterno - &IcmsInterestadual),2)

 
        endcase

        If &Diferenca < 0
            &Diferenca = 0
        EndIf

        &SdtNfProItem.NfiVlrIcmsDest = Round( (&Diferenca * &percentual) / 100 ,2)  // Valor do ICMS para o destinatario sem o valor do Fundo de Combate a Pobreza
        &SdtNfProItem.NfiVlrIcmsOri  = &Diferenca - &SdtNfProItem.NfiVlrIcmsDest         // Valor do ICMS para o remetente

        If &SdtNfProItem.NfiVlrIcmsOri < 0
            &SdtNfProItem.NfiVlrIcmsOri = 0
        EndIf

    EndIf                   
endsub


sub 'DividePed'
    
    &ColSdtPdi.Clear()
    for each
        where CreCod = &PedCreCod

        &CreVlr = CreVlr
        &CreTp  = CreTp

    endfor

    if &PedFrete > 0
        &PedFrete1 = round(&PedFrete * &CreVlr / 100,2)
        &PedFrete2 = &PedFrete - &PedFrete1
    endif

    if &PedDsc > 0
        &PedDsc1 = round(&PedDsc * &CreVlr / 100,2)
        &PedDsc2 = &PedDsc - &PedDsc1
    endif

    if &PedAcr > 0
       &PedAcr1 = round(&PedAcr * &CreVlr / 100,2)
       &PedAcr2 = &PedAcr - &PedAcr1
    endif

    &cont2 = 0

    for each PedCod, PdiSeq
        where PedCod = &PedCod
            
            &cont2 += 1
           
            If &PedTotVlrPro > 0
                &Desconto   = Round((&PedDsc1   / &PedTotVlrPro * PdiTotVlr),2)
                &Acrescimo  = Round((&PedAcr1   / &PedTotVlrPro * PdiTotVlr),2)
                &Frete      = Round((&PedFrete1 / &PedTotVlrPro * PdiTotVlr),2)

                &Desconto2   = Round((&PedDsc2   / &PedTotVlrPro * PdiTotVlr),2)
                &Acrescimo2  = Round((&PedAcr2   / &PedTotVlrPro * PdiTotVlr),2)
                &Frete2      = Round((&PedFrete2 / &PedTotVlrPro * PdiTotVlr),2)
            EndIF

            &totdsc     += &Desconto
            &totacr     += &Acrescimo
            &totfrete   += &Frete

            &totdsc2     += &Desconto2
            &totacr2     += &Acrescimo2
            &totfrete2   += &Frete2

            // Faz ajuste da diferença no último produto da parte 2
            If &cont2 = &PedContQtd
                If (&PedDsc1 + &PedDsc2) <> (&totdsc + &totdsc2)
                    &Desconto2 += (&PedDsc1 + &PedDsc2) - (&totdsc + &totdsc2)
                EndIf

                If (&PedAcr1 + &PedAcr2) <> (&totacr + &totacr2)
                    &Acrescimo2 += (&PedAcr1 + &PedAcr2) - (&totacr + &totacr2)
                EndIf

                If (&PedFrete1 + &PedFrete2) <> (&totfrete + &totfrete2)
                    &Frete2 += (&PedFrete1 + &PedFrete2) - (&totfrete + &totfrete2)
                EndIf
            EndIf
            // ------------------------------------------------------- FIM Rateio de Desconto, Acréscimo e Frete (28/11/2017)


            if &CreTp = 'Q' // QUANTIDADE

                   &Quantidade = PdiQtd * (&CreVlr / 100)  
    
                   if PdiPrdTpQtd = 2
                      &Quantidade = round(&Quantidade,0)
                   endif

                   &Quantidade2 = PdiQtd - &Quantidade

                   if &Quantidade > 0
                       &SdtPdi = new SdtPdi()
                       &SdtPdi.PdiSeq     = PdiSeq
                       &SdtPdi.PdiPrdCod  = PdiPrdCod
                       &SdtPdi.PdiPrdDsc  = PdiPrdDsc
                       &SdtPdi.PdiPrdUnd  = PdiPrdUnd
                       &SdtPdi.PdiVlrUnt  = PdiVlrUntDsc
                       &SdtPdi.PdiQtd     = &Quantidade
                       &SdtPdi.PdiTotVlr  = round(&Quantidade * &SdtPdi.PdiVlrUnt,2)

                       if &Quantidade2 = 0
                          &SdtPdi.PdiFrete = &Frete + &Frete2
                          &SdtPdi.PdiAcr   = &Acrescimo + &Acrescimo2
                          &SdtPdi.PdiDesconto = &Desconto + &Desconto2
                       else
                          &SdtPdi.PdiFrete = &Frete
                          &SdtPdi.PdiDesconto = &Desconto
                          &SdtPdi.PdiAcr = &Acrescimo
                       endif

                       &SdtPdi.PdiCfopSeq  = PdiCfopSeq
                       &SdtPdi.PdiCfopCod  = PdiCfopCod
                       &SdtPdi.PdiCfopMovFin = PdiCfopMovFin
                       &SdtPdi.PdiCfopMovStq = PdiCfopMovStq
                       &SdtPdi.PdiNfEntrada = PdiNfEntrada

                       &SdtPdi.Seq         = 1
                       &ColSdtPdi.Add(&SdtPdi)
                   endif

                   if &Quantidade2 > 0
                       &SdtPdi = new SdtPdi()
                       &SdtPdi.PdiSeq     = PdiSeq
                       &SdtPdi.PdiPrdCod  = PdiPrdCod
                       &SdtPdi.PdiPrdDsc  = PdiPrdDsc
                       &SdtPdi.PdiPrdUnd  = PdiPrdUnd
                       &SdtPdi.PdiVlrUnt  = PdiVlrUntDsc
                       &SdtPdi.PdiQtd     = &Quantidade2
                       &SdtPdi.PdiTotVlr  = round(&Quantidade2 * &SdtPdi.PdiVlrUnt,2)
                       &SdtPdi.PdiFrete = &Frete2
                       &SdtPdi.PdiDesconto = &Desconto2
                       &SdtPdi.PdiAcr = &Acrescimo2
                       &SdtPdi.PdiCfopSeq  = PdiCfopSeq
                       &SdtPdi.PdiCfopCod  = PdiCfopCod
                       &SdtPdi.PdiCfopMovFin = PdiCfopMovFin
                       &SdtPdi.PdiCfopMovStq = PdiCfopMovStq
                       &SdtPdi.PdiNfEntrada = PdiNfEntrada
                       &SdtPdi.Seq         = 2
                       &ColSdtPdi.Add(&SdtPdi)
                   endif


            else

                   &Valor = round(PdiVlrUnt * &CreVlr / 100,3)
                   &Valor2 = PdiVlrUnt - &Valor
                   &PdiDsc = 0

                   if PdiPerDsc > 0
                      &PdiDsc += round(&Valor * (PdiPerDsc / 100),2)
                      &dsc = round(&Valor * (PdiPerDsc / 100),2)
                      &Valor = &Valor - &dsc
                   endif

                   if PdiPerDsc2 > 0
                      &PdiDsc += round(&Valor * (PdiPerDsc2 / 100),2)
                      &dsc = round(&Valor * (PdiPerDsc2 / 100),2)
                      &Valor = &Valor - &dsc
                   endif

                   if PdiPerDsc3 > 0
                      &PdiDsc += round(&Valor * (PdiPerDsc3 / 100),2)
                      &dsc = round(&Valor * (PdiPerDsc3 / 100),2)
                      &Valor = &Valor - &dsc
                   endif

                   if PdiPerDsc4 > 0
                      &PdiDsc += round(&Valor * (PdiPerDsc4 / 100),2)
                      &dsc = round(&Valor * (PdiPerDsc4 / 100),2)
                      &Valor = &Valor - &dsc
                   endif

                   if PdiPerDsc5 > 0
                      &PdiDsc += round(&Valor * (PdiPerDsc5 / 100),2)
                      &dsc = round(&Valor * (PdiPerDsc5 / 100),2)
                      &Valor = &Valor - &dsc
                   endif                   

                   &SdtPdi = new SdtPdi()
                   &SdtPdi.PdiSeq      = PdiSeq
                   &SdtPdi.PdiPrdCod   = PdiPrdCod
                   &SdtPdi.PdiPrdDsc   = PdiPrdDsc
                   &SdtPdi.PdiVlrUnt   = &Valor
                   &SdtPdi.PdiQtd      = PdiQtd
                   &SdtPdi.PdiPrdUnd   = PdiPrdUnd
                   &SdtPdi.PdiTotVlr   = round(PdiQtd * &SdtPdi.PdiVlrUnt,2)
                   &SdtPdi.PdiFrete    = &Frete
                   &SdtPdi.PdiDesconto = &Desconto
                   &SdtPdi.PdiAcr      = &Acrescimo
                   &SdtPdi.PdiCfopSeq  = PdiCfopSeq
                   &SdtPdi.PdiCfopCod  = PdiCfopCod
                   &SdtPdi.PdiCfopMovFin = PdiCfopMovFin
                   &SdtPdi.PdiCfopMovStq = PdiCfopMovStq
                   &SdtPdi.PdiNfEntrada = PdiNfEntrada
                   &SdtPdi.Seq         = 1
                   &ColSdtPdi.Add(&SdtPdi)

                   if PdiPerDsc > 0
                      &PdiDsc += round(&Valor2 * (PdiPerDsc / 100),2)
                      &dsc = round(&Valor2 * (PdiPerDsc / 100),2)
                      &Valor2 = &Valor2 - &dsc
                   endif

                   if PdiPerDsc2 > 0
                      &PdiDsc += round(&Valor2 * (PdiPerDsc2 / 100),2)
                      &dsc = round(&Valor2 * (PdiPerDsc2 / 100),2)
                      &Valor2 = &Valor2 - &dsc
                   endif

                   if PdiPerDsc3 > 0
                      &PdiDsc += round(&Valor2 * (PdiPerDsc3 / 100),2)
                      &dsc = round(&Valor2 * (PdiPerDsc3 / 100),2)
                      &Valor2 = &Valor2 - &dsc
                   endif

                   if PdiPerDsc4 > 0
                      &PdiDsc += round(&Valor2 * (PdiPerDsc4 / 100),2)
                      &dsc = round(&Valor2 * (PdiPerDsc4 / 100),2)
                      &Valor2 = &Valor2 - &dsc
                   endif

                   if PdiPerDsc5 > 0
                      &PdiDsc += round(&Valor2 * (PdiPerDsc5 / 100),2)
                      &dsc = round(&Valor2 * (PdiPerDsc5 / 100),2)
                      &Valor2 = &Valor2 - &dsc
                   endif

                   &SdtPdi = new SdtPdi()
                   &SdtPdi.PdiSeq      = PdiSeq
                   &SdtPdi.PdiPrdCod   = PdiPrdCod
                   &SdtPdi.PdiPrdDsc   = PdiPrdDsc
                   &SdtPdi.PdiVlrUnt   = &Valor2
                   &SdtPdi.PdiQtd      = PdiQtd
                   &SdtPdi.PdiPrdUnd   = PdiPrdUnd
                   &SdtPdi.PdiTotVlr   = round(PdiQtd * &SdtPdi.PdiVlrUnt,2)
                   &SdtPdi.PdiFrete    = &Frete2
                   &SdtPdi.PdiDesconto = &Desconto2
                   &SdtPdi.PdiAcr      = &Acrescimo2
                   &SdtPdi.PdiCfopSeq  = PdiCfopSeq
                   &SdtPdi.PdiCfopCod  = PdiCfopCod
                   &SdtPdi.PdiCfopMovFin = PdiCfopMovFin
                   &SdtPdi.PdiCfopMovStq = PdiCfopMovStq
                   &SdtPdi.PdiNfEntrada = PdiNfEntrada
                   &SdtPdi.Seq         = 2
                   &ColSdtPdi.Add(&SdtPdi)

            endif

    endfor

endsub


sub'Cubagem'
    &NfsQtd = 0
    &NfsCubagem = 0

    if &PrdMtlEmb > 0
       &NfsQtd = round(&PdiQtd / &PrdMtlEmb,2)
    endif

    for each
        where EmbCod = &PrdEmbCod   
        
            &NfsCubagem = EmbCub * &NfsQtd    
    endfor
endsub


Sub 'UltNfCompra'
    &NicBseClcSt = 0
    &NicVlrSt = 0
    &NicAlqIcmsSt = 0
    &NicVlrUnt = 0
    &NicQtd = 0
    &NicVlrIcms = 0

    For Each (NfcDtaEms)
        Where NicPrdCod = &PdiPrdCod
        Where NicBseClcSt > 0
        Where NfcSts = 'F' // Finalizado

            &NicBseClcSt = NicBseClcSt
            &NicVlrSt = NicVlrSt
            &NicAlqIcmsSt = NicAlqIcmsSt
            &NicVlrUnt = NicVlrUnt
            &NicQtd = NicQtd    
        
            If NicVlrIcms > 0
                &NicVlrIcms = NicVlrIcms // Regime normal
            Else
                &NicVlrIcms = NicVlrCredIcms // Simples nacional
            EndIf

            Exit
    EndFor
EndSub

Sub'CondPgtoIpi'

&CondRatIPI = NullValue(&CondRatIPI)

For Each
 Where CondCod = &PedCondCod

   &CondRatIPI = CondRatIPI

EndFor

EndSub

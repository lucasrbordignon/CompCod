Event 'nfe'
    if not null(PedCod)
        Call(PVerStsPed, &Logon, PedCod, &PedSts2)
        if &PedSts2 = 1 or &PedSts2 = 5
            if PedTotVlr > 0 or PedTipTrnNOP <> 1 // Se for Tipo de Transação = 1 (Vendas) Não permite valor unitário zerado, os outros tipos permitem                  
                If PedCreCod <> &EmpCreCodAContabilizar or Null(PedCreCod)
                    Confirm('Deseja Finalizar o Pedido e Gerar a Nota Fiscal ?')
                    if confirmed() 
                        If PedTipTrn <> &EmpTipTrnCodOS // Não é uma Transação de  Ordem de Serviço
                            &PedCod3 = PedCod
                            Do 'VerificaCfopItens'
                            If &CfopItensOK = 'S' // Todos os itens do pedido estão com a CFOP informada 
                                &PedTrpCod = PedTrpCod
                                Do 'Transp'

                                If &Logon.EmpClienteNa12 = 19 and &TrpAutonoma = 'S'
                                    WVlrM3Frt.Call(&Logon,PedCod)
                                EndIf

                                Call(PGeraNota, &Logon,PedCod,'N',0)

                                If &Logon.EmpClienteNa12 = 13 and PedTipTrnNOP = 3 // Se for JCK e Devolução de Venda
                                    Call(PVerStsPed, &Logon, PedCod, &PedSts2)
                                    If &PedSts2 = 2
                                        &Mensagem = 'Deseja gerar um crédito de R$ ' + Trim(ToFormattedString(PedTotVlr)) + ' para o cliente ' + Trim(PedCliNom) + '?'
                                        Confirm(&Mensagem, N)
                                        If Confirmed()
                                            &CredCliCod = PedCliCod
                                            &CredVlr = PedTotVlr
                                            Do 'GerarCredito'
                                        EndIf
                                    EndIf
                                EndIf
                            Else
                                Msg('O Pedido possui itens sem CFOP cadastrada, favor verificar para poder gerar a NF-e!')
                            EndIf
                        Else // é uma Transação de Ordem de Serviço
                            If PedCreCod = &EmpCreCodContabil or Null(PedCreCod)
                                WNFSer.Call(&Logon,PedCod,&Pgmname)
                            Else
                                Msg('A Credibilidade do Pedido ' + trim(str(PedCod)) + ' é incompatível com o tipo de finalização, favor verificar!')
                            EndIf
                        EndIf
                    endif
                Else
                    Msg('A Credibilidade do Pedido ' + trim(str(PedCod)) + ' é incompatível com o tipo de finalização, favor verificar!')
                EndIf
            else
                msg('Valor do Pedido não pode ser zero')
            endif
        else
            if &PedSts2 = 2
              confirm('Deseja Gerar a Nota Fiscal ?')
              if confirmed()
      
                 &Flag = 'N'
                 &PedCod3 = PedCod
                 do'locped'
                 if &Flag = 'N'
              
                    Call(PGeraNota, &Logon,PedCod,'S',1)
      
                 else
      
                    msg('Já existe uma Nota Fiscal para este pedido')
      
                 endif
      
              endif
            else
              msg('Situação do Pedido não permite esta operação')
            endif
        endif
    else
        msg('Favor selecionar um pedido!')
    endif

    refresh
EndEvent  // 'nfe'
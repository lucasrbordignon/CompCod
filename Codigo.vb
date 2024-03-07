
Msg("Montagem Txt... Nota Fiscal Eletrônica",status)

for each
   Where EmpCod = &Logon.EmpCod
      
      &EmpModImp = EmpModImp
      &EmpProcEmi = EmpProcEmi
      &EmpIdAmb    = EmpIdAmb  

      if not null(EmpVerLei)
         &EmpVerLei    = EmpVerLei
      else
         &EmpVerLei    = '4.00'
      endif

      if not null(EmpVerEmi)
         &EmpVerEmi    = EmpVerEmi
      else
         &EmpVerEmi    = '4.00.00'
      endif

      &EmpCRT = EmpCRT
      &EmpPerCredIcms = EmpPerCredIcms 
      &EmpTipoLote = EmpTipoLote

      &EmpNom = EmpNom
      &EmpNomFan = EmpNomFan
      &EmpCnpj = EmpCnpj
      &EmpIE = EmpIE
      &EmpIEise = EmpIEise

      if &EmpIEise = 'S'
         &EmpIE = 'ISENTO'
      endif

      &EmpEnd = EmpEnd
      &EmpEndNum = EmpEndNum
      &EmpEndComp = EmpEndComp
      &EmpEndBai = EmpEndBai
      &EmpCep = EmpCep
      &EmpCidade = EmpCidade
      &EmpUf = EmpUf
      &EmpFone = EmpFone
      &EmpTpEndSeq = EmpTpEndSeq

      do'tipo'

      &EmpInscMun = EmpInscMun
      &EmpCnae = EmpCnae

      &EmpCnae = strreplace(&EmpCnae,'.','')
      &EmpCnae = strreplace(&EmpCnae,'-','')
      &EmpCnae = strreplace(&EmpCnae,'/','')
      &EmpCnae = strreplace(&EmpCnae,'\','')
      &EmpCnae = strreplace(&EmpCnae,',','')

      &EmpCnpj = strreplace(&EmpCnpj,'.','')
      &EmpCnpj = strreplace(&EmpCnpj,'-','')
      &EmpCnpj = strreplace(&EmpCnpj,'/','')
      &EmpCnpj = strreplace(&EmpCnpj,'\','')
      &EmpCnpj = strreplace(&EmpCnpj,',','')

      &EmpIE = strreplace(&EmpIE,'.','')
      &EmpIE = strreplace(&EmpIE,'-','')
      &EmpIE = strreplace(&EmpIE,'/','')
      &EmpIE = strreplace(&EmpIE,'\','')
      &EmpIE = strreplace(&EmpIE,',','')

      &EmpCep = strreplace(&EmpCep,'-','')
      &EmpCep = strreplace(&EmpCep,'.','')
      &EmpCep = strreplace(&EmpCep,' ','')

      &EmpFone = strreplace(&EmpFone,'-','')
      &EmpFone = strreplace(&EmpFone,'(','')
      &EmpFone = strreplace(&EmpFone,')','')
      &EmpFone = strreplace(&EmpFone,' ','')

      &EmpIdent = EmpIdent         
endfor

for each
   where UfCod = &EmpUf
        &UfCodUf = UfCodUf
endfor

for each
    where UfCod = &EmpUf
    where CidNom = &EmpCidade
        &Empcidcod = CidCod
endfor

for each
   where UfCod = &EmpUf

       if not null(UfInscSub)
          &UfInscSub = UfInscSub
          &UfInscSub = StrReplace(&UfInscSub,'-','')
          &UfInscSub = StrReplace(&UfInscSub,',','')
          &UfInscSub = StrReplace(&UfInscSub,'*','')
          &UfInscSub = StrReplace(&UfInscSub,'.','')
          &UfInscSub = StrReplace(&UfInscSub,'#','')
          &UfInscSub = StrReplace(&UfInscSub,'/','')
          &UfInscSub = StrReplace(&UfInscSub,' ','')
          &UfInscSub = Trim(&UfInscSub)
       endif 
endfor
      

&Arquivo  = Trim(&arquivo) + Trim('\ ') 
&Arquivo  += 'NotaFiscal.txt' 

&ret = Deletefile(&Arquivo) // Apaga Arquivo ja existente

//Criando Arquivo
&ret = DFWOpen(&Arquivo,'','',0)
&Linha  = 'NOTAFISCAL|' + Trim(str(&SdtNota.Count))

//Gerando Linha no Arquivo
&ret = DFWPTxt(&Linha,1000)
&ret = DFWNext()

do 'montaArquivo'

//Finalizando Arquivo
&ret = DFWClose()


sub 'MontaArquivo'
&TiraBarra = 'S'

for &SdtNotaItem in &SdtNota

   for each
      where NfsNum = &SdtNotaItem.NfsNum
      where NfsSer = &SdtNotaItem.NfsSer
            

            &linha  = 'A|'+trim(&EmpVerLei)      //ACERTAR A VERSAO NO BANCO DE DADOS ATRIBUTO NFE002, COMENTADO ACIMA
            &linha  += '|'                       // Informar em branco este campo
            &ret = DFWPTxt(&Linha,1000)
            &ret = DFWNext()
            //***************************************************************

            &NfsNumPed = NfsNumPed
            &NfsCfopSeq = NfsCfopSeq
            &NfsCliCod = NfsCliCod
            &NfsForCod = NfsForCod
            &NfsNum    = NfsNum
            &NfsSer    = NfsSer
            &NfsTipTrnNOP  = NfsTipTrnNOP            
            &NfsConsFinal = NfsConsFinal             

            // NfsSts = 'E' // Comentado dia 12/02/2019: Só vai ficar com status enviada quando importar o retorno do emissor.
            
            do'DadosPedido'
            do'opr'
            do'cliente'

            &Dia = Padl(Trim(Str(Day(NfsDtaEms),2,0)),2,'0')
            &Mes = Padl(Trim(Str(Month(NfsDtaEms),2,0)),2,'0')
            &Ano = str(Year(NfsDtaEms),4,0)

            csharp System.TimeZone localZone = System.TimeZone.CurrentTimeZone;
            csharp [!&Horario!] = localZone.DaylightName;

            if &Horario = 'Horário brasileiro de verão'
                &DtaEmi =  &Ano + "-" + &Mes + '-' + &Dia + 'T' +&Time + '-02:00'
            else
                &DtaEmi =  &Ano + "-" + &Mes + '-' + &Dia + 'T' +&Time + '-03:00'
            endif

            &Dia = Padl(Trim(Str(Day(NfsDtaSai),2,0)),2,'0')
            &Mes = Padl(Trim(Str(Month(NfsDtaSai),2,0)),2,'0')
            &Ano = str(Year(NfsDtaSai),4,0)

            If Not Null(NfsHraSai)
                &HoraSaida = TtoC(NfsHraSai)
            Else
                &HoraSaida = &Time
            EndIf

            if &Horario = 'Horário brasileiro de verão'
               &DtaSai =  &Ano + "-" + &Mes + '-' + &Dia + 'T' + Trim(&HoraSaida) + '-02:00'
            else
               &DtaSai =  &Ano + "-" + &Mes + '-' + &Dia + 'T' + Trim(&HoraSaida) + '-03:00'
            endif

            &NfsDtaCont = NfsDtaCont   

            &Dia             =  NullValue(&Dia)
            &Mes             =  NullValue(&Mes)
            &Ano             =  NullValue(&Ano)
            &Data_Formatada  =  NullValue(&Data_Formatada)
        
            &Dia             =  Padl(Trim(Str(Day(&NfsDtaCont),2,0)),2,'0')
            &Mes             =  Padl(Trim(Str(Month(&NfsDtaCont),2,0)),2,'0')
            &Ano             =  Str(Year(&NfsDtaCont),4,0)
        
            &Data_Formatada  =  &Ano + "-" + &Mes + '-' + &Dia    

            do case
               case &EmpUf = &CliUfCod
                   &iddest = '1'
               case &EmpUf <> &CliUfCod and &CliUfCod <> 'EX'
                   &iddest = '2'
               case &CliUfCod = 'EX'
                   &iddest = '3'
            otherwise
                   &iddest = '1'
            endcase

            &NfsSer2 = val(NfsSer) 

            //identificadores da NF-e
            &linha = 'B|'+trim(str(&UfCodUf)) // N  2    Cod. UF
            &linha += '|'                                      // N  9    Chave de acesso
            &linha += '|'+trim(NfsDescCfop)                       // C 60    Descr. Nat. op.
            &linha += '|'+trim('55')                          // C  2    Cod. Mod. NF
            &linha += '|'+trim(str(&NfsSer2))                // N  3    Serie Doc. Fiscal
            &linha += '|'+trim(str(NfsNum))       // N  9    Num. Doc. Fiscal
            &linha += '|'+trim(&DtaEmi)                       // D       Data de emissao
            &linha += '|'+trim(&DtaSai)                  // D       Data de saida
            &linha += '|'+&iddest                        // N       IDENTIFICADOR DE DESTINO DA OPERAÇÃO
            &linha += '|'+trim(str(NfsTpNf,1))              // N   1   Tipo Doc. Fiscal
            &linha += '|'+Padl(trim(str(&Empcidcod,7)),7,'0')// N   7   Cod. Municipio
            &linha += '|'+trim(str(&EmpModImp,1))          // N   1   Formanto de impressao do Danfe 
            &linha += '|'+trim(str(&EmpIdAmb,1))          // N   1   Forma de emissao da NF-e
            &linha += '|'                                    // N   1   Digito verificador da chave de acesso da NF-e
            &linha += '|'+trim(str(&EmpIdent,1))             // N   1   Identificacao do Ambiente
            &linha += '|'+trim(str(&CfopFinEmi,1))              // N   1   Finaldidade de emissao da NF-e
            &linha += '|'+trim(str(&EmpProcEmi,1))                      // N   1   Processo da emissao da NF-e
            &linha += '|'+trim(&EmpVerEmi)                 // C  20   Versao do Processo da emissao da NF-e - 

            if &EmpProcEmi = 2 // se a nota estiver em contingencia         

                &Hora = Ttoc(NfsHraCont)

                if &Horario = 'Horário brasileiro de verão'
                   &DH_Cont = &Data_Formatada + 'T' + &Hora + '-02:00'
                   &DH_cont = trim(&DH_cont)
                else
                   &DH_Cont = &Data_Formatada + 'T' + &Hora + '-03:00'
                   &DH_cont = trim(&DH_cont)
                endif
                
                &linha += '|'+trim(&DH_Cont)                      // D      Data e hora de entrada em contingencia (AAAA-MM-DDTHH:MM:SS) (NEW)
                &linha += '|'+trim(NfsJustCont)                   // D      Justificativa da contingencia (New)

            else
                &linha += '|'                                     //  D      Data e hora de entrada em contingencia (AAAA-MM-DDTHH:MM:SS) (NEW)
                &linha += '|'                                     //  D      Justificativa da contingencia (New)
            endif            

            if NfsConsFinal = 'S'//Consumidor Final
               &Linha += '|1'
            else 
               &Linha += '|0'
            endif

            &Linha += '|' + Trim(Str(&PedIndPres))              // N 1  indPres         Indicador de presença do comprador no estabelecimento comercial no momento da operação 
            &Linha += '|' + Trim(Str(&PedIndIntermed))          // N 1  indIntermed     Indicador de Intermediador/Marketplace
            &Linha += '|' + Trim(&PedCnpjIntermed)              // C 14 CNPJIntermed    CNPJ do Intermediador da Negociação
            &Linha += '|' + Trim(&PedIdCadIntTran)              // C 60 idCadIntTran    Identidicador do cadastro no intermediador
  
            do 'tira_car'

        //***********************************************************************
            for each
               defined by NfChRef

               if not null(NfChRef)
        
                   &linha = 'B13|'+trim(NfChRef)  //  N 44       Chave de acesso da NF-e         esta C 44 no sistema 
                   do 'tira_car'
        
               endif
            endfor 
        //************************************************************************


        //***********************************************************************
            For Each NfRefSeq
                Defined by NfRefSeq

                If NfRefTipoNfRef = 2 //  Informação da NF modelo 1/1A referenciada
                    &linha  = 'B14|' + trim(str(NfRefUfCodIbge))      // N 2     Código da UF do emitente
                    &linha += '|' + trim(NfRefAnoMes)                 // N 4     Ano e Mês de emissão da NF-e

                    &NfRefCnpj = NfRefCnpj
                    &NfRefCnpj = strreplace(&NfRefCnpj,'.','')
                    &NfRefCnpj = strreplace(&NfRefCnpj,'-','')
                    &NfRefCnpj = strreplace(&NfRefCnpj,'/','')

                    &linha += '|' + trim(&NfRefCnpj)                  // N 14    CNPJ do emitente
                    &linha += '|' + trim(NfRefMod)                    // N 2     Modelo do Documento Fiscal
                    &linha += '|' + trim(NfRefSerie)                  // N 3     Série do Documento Fiscal
                    &linha += '|' + trim(str(NfRefNum))               // N 9     Número do Documento Fiscal
                    do 'tira_car'                
                EndIf

                If NfRefTipoNfRef = 3 //  Informações da NF de produtor rural referenciada
                    &linha  = 'B20A|' + trim(str(NfRefUfCodIbge))     // N 2     Código da UF do emitente
                    &linha += '|' + trim(NfRefAnoMes)                 // N 4     Ano e Mês de emissão da NF-e
                    &linha += '|' + trim(NfRefIe)                     // N 4     IE do emitente
                    &linha += '|' + trim(NfRefMod)                    // N 2     Modelo do Documento Fiscal
                    &linha += '|' + trim(NfRefSerie)                  // N 3     Série do Documento Fiscal
                    &linha += '|' + trim(str(NfRefNum))               // N 9     Número do Documento Fiscal
                    do 'tira_car'     
          
                    &NfRefCnpj = NfRefCnpj
                    &NfRefCnpj = strreplace(&NfRefCnpj,'.','')
                    &NfRefCnpj = strreplace(&NfRefCnpj,'-','')
                    &NfRefCnpj = strreplace(&NfRefCnpj,'/','')

                    &NfRefCpf = NfRefCpf
                    &NfRefCpf = strreplace(&NfRefCpf,'.','')
                    &NfRefCpf = strreplace(&NfRefCpf,'-','')
                    &NfRefCpf = strreplace(&NfRefCpf,'/','')

                    If NfRefTipoDoc = 1
                        &linha  = 'B20D|' + trim(&NfRefCnpj)           // N 14     CNPJ do emitente
                        do 'tira_car'                     
                    Else
                        &linha  = 'B20E|' + trim(&NfRefCpf)            // N 11     CPF do emitente
                        do 'tira_car'                     
                    EndIF  
                EndIf
            EndFor
        //************************************************************************


        //************************************************************************

             //Emitente
            &linha = 'C|'+trim(&EmpNom)     // C  60   Razao social ou nome do emitente
            &linha += '|'+trim(&EmpNomFan)  // c  60   Nome fantasia
            &linha += '|'+trim(&EmpIE)     // C  14   I.E.
            &linha += '|'+trim(&UfInscSub)                   // C  14   I.E. do subst. trib.
            &linha += '|'+trim(&EmpInscMun)     // C  15   Inscr. municipal
            &linha += '|'+trim(&EmpCnae)      // C   7   Cnae fiscal
            &linha += '|'+trim(&EmpCRT)// N   1   Codigo do Regime Tributário (New)
            do 'tira_car'
            
            if not null(&EmpCnpj)

               &linha = 'C02|'+trim(&EmpCnpj)  // C  14   Cnpj do emitente
               do 'tira_car'
               
            endif

            //*********************************************************
            //Endereco
            &linha = 'C05|'+trim(&TipoEnd)+'  '+trim(&EmpEnd)   // C  60   Logradouro
            &linha += '|'+trim(&EmpEndNum)  // C  60   Numero
            &linha += '|'+trim(&EmpEndComp)  // C  60   Complemento
            &linha += '|'+trim(&EmpEndBai)     // C  60   bairro
            &linha += '|'+Padl(trim(str(&Empcidcod,7)),7,'0')  // N   7   Cod. municipio
            &linha += '|'+trim(&EmpCidade)     // C  60   Nome do municipio
            &linha += '|'+trim(&EmpUf)      // C   2   Sigla da UF
            &linha += '|'+Padl(trim(&EmpCep),8,'0')  // N   8   Cod. Cep 
            &linha += '|'+trim('1058')      // N   4   cod. pais
            &linha += '|'+trim('Brasil')    // C  60   Nome do pais
            &linha += '|'+trim(&EmpFone)     // N  10   telefone
            do 'tira_car'
             
            //***********************************************
            
            // Destinatario
            if &EmpIdent = 1
               &linha = 'E|'+trim(&CliNom)       // C  60   Razao Social ou nome do destinatario
            else
               &linha = 'E|NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL'
            endif

            if &EmpIdent = 1
                If Null(&CliInsProdRural) // Não é produtor rural
                    &linha += '|'+trim(&CliIes)    // C  14   I.E.  
                Else // É um produtor rural
                    &linha += '|'+trim(&CliInsProdRural)    // C  14   I.E. 
                EndIf
            else
               &Linha += '|'
            endif

            &linha += '|'+trim(&CliInsSuf)    // C   9   Inscr. suframa
            &linha += '|'+trim(&CliEmail)      // C  200   E-mail  

            do case
               case &CliIeIse = 'I'
                   &Linha += '|2'
               case &CliIeIse = 'C'
                   &Linha += '|1'
               case &CliIeIse = 'N'
                   &Linha += '|9'
           endcase
                        
            &linha += '|'+trim(&CliIM)    // C   9   Inscr. Municipal

            do 'tira_car'         
            
            if &CliTp = 'J'
               if &EmpIdent = 1
                  &linha = 'E02|'+trim(&CliCnpj)  // C  14   cnpj
                  do 'tira_car'
               else
                  &linha = 'E02|99999999000191'  // C  14   cnpj
                  do 'tira_car'
               endif   
            endif

            if &CliTp = 'F'
               &linha = 'E03|'+trim(&CliCpf)   // C  11   cpf
               do 'tira_car'
            endif

            if &CliTp = 'E'
               &linha = 'E04|'+trim(&CliPassaporte)   // C  11   cpf
               do 'tira_car'
            endif

            //Endereço
            &linha = 'E05|'+trim(&CliTipoEnd)+'  '+trim(&CliEnd)                   // C  60   logradouro
            &linha += '|'+trim(&CliEndNum)                         // C  60   numero
            &linha += '|'+trim(&CliEndComp)                         // C  60   Complemento
            &linha += '|'+trim(&CliEndBai)                            // C  60   Bairro
            &linha += '|'+Padl(trim(str(&CliCidCod,7)),7,'0')  // N   7   Cod. Municipio
            &linha += '|'+trim(&CliCidNom)                         // C  60   Nome municipio
            &linha += '|'+trim(&CliUfCod)                          // C   2   Sigla da uf
            &linha += '|'+trim(&CliCep)                                          // N   8 cep do Pais
            &linha += '|'+trim(str(&CliPaiCod))                            // N   4   codigo do pais)
            &linha += '|'+trim(&CliPaiNom)                           // C  60   Nome do pais
            //&linha +='|'                                         // colocado esta barra pois foi comentada a linha abaixo
            
            if not null(&CliFone) and &CliFone <> '(  )     -    '
               &linha += '|'+trim(&CliFone)                         // N   10   telefone   // campo opcional
            else
               &linha += '|'+trim(&CliCel)                         // N   10   telefone   // campo opcional
            endif

            do 'tira_car'
            
            if not null(&CliCnpjEndEnt) or not null(&CliCpfEndEnt)

               &linha = 'G|'+trim(&CliTipoEndEnt)+'  '+trim(&CliEndEnt)  // C  60   logradouro
               &linha += '|'+trim(&CliEndNumEnt)  // C  60   numero
               &linha += '|'+trim(&CliEndComplEnt)  // C  60   Complemento
               &linha += '|'+trim(&CliEndBaiEnt)     // C  60   Bairro
               &linha += '|'+Padl(trim(str(&CliCidCodEnt,7)),7,'0')  // N   7   Cod. municipio
               &linha += '|'+Trim(&CliCidNomEnt)  // C  60   Nome do municipio
               &linha += '|'+trim(&CliUfCodEnt)   // C   2   Sigla da uf
               &linha += '|'  + trim(&CliRzEndEnt)                                                          // C  60      (xNome)          Razão Social ou Nome do Recebedor
               &linha += '|'  + trim(&cliCepEnt)                                                            // C   8      (CEP)            Código do CEP
               &linha += '|'  + trim(str(&CliPaiCodEnt))                                                    // N   4      (cPais)          Código do País
               &linha += '|'  + trim(&CliPaiNomEnt)                                                         // C  60      (Pais)           Nome do País         
               &linha += '|'  + trim(&CliFoneEndEnt)                                                        // N  14      (Fone)           Telefone                    
               &linha += '|'  + trim(&CliEmailEndEnt)                                                       // C  60      (Email)          Endereço de e-mail do Recebedor                    
               &linha += '|'  + trim(&CliIeEndEnt)                                                          // C  14      (IE)             Inscrição Estadual do Estabelecimento Recebedor           

               do 'tira_car'
               if &CliTp2 = 'J'
                    &linha = 'G02|'+trim(&CliCnpjEndEnt)   //   C    14       CNPJ
                else
                    &linha = 'G02a|'+trim(&CliCpfEndEnt)   //   C    11       CPF
               endif
               do 'tira_car'
               

            endif
            //***********************************************************************************

            //Pessoas autorizadas a realizar o backup do xml
            do'Autorizações'


       // Detalhamento de produtos e serviços - maximo 990

       for each 
           defined by NfiQtd

               &PrdCod = NfiPrdCod
               &NfiSeq = NfiSeq
               &NfiCfopSeq = NfiCfopSeq
               do'cfop'

               &Valor = round((NfiQtd*NfiVlrUnt),2)
            
               if null(NfiObs)
                  &linha = 'H|'+trim(str(NfiSeq,3))+'||'            // N   3   Numero do item   
               else
                  &obs = NfiObs // +' - ' +&obs.Trim()
                  &obs = substr(&obs,1,500)
                  &linha = 'H|'+trim(str(NfiSeq,3))+'|'+&obs.Trim()+'|'      // N   3   Numero do item   
               endif

               do 'tira_car'

               &NfiPrdNcmCod = NfiPrdNcmCod
               &NfiPrdNcmCod = strreplace(&NfiPrdNcmCod,'.','')
               &NfiPrdNcmCod = strreplace(&NfiPrdNcmCod,' ','')
               &NfiPrdNcmCod = strreplace(&NfiPrdNcmCod,'-','')                   

               &linha = 'I|'+trim(NfiPrdCod)            // C  60    Cod. do produto ou serviço 

               If Not Null(&PrdcEan)
                    &linha += '|'+trim(&PrdcEan)             // C  14    (cEAN) GTIN (código. de barras)
               Else
                    &linha += '|SEM GTIN'
               EndIf

               &linha += '|'+trim(substr(NfiPrdDsc,1,100))  // C 120    Descriçao do produto
               &linha += '|'+trim(&NfiPrdNcmCod)                                // C   8    Codigo NCM
                
               If null(NfiPrdNcmEx) or NfiPrdNcmEx = '00'
                   &linha += '|'                                // C   3    EX_TIPI
               Else
                   &linha += '|' + trim(NfiPrdNcmEx)                               
               EndIf
                                  
               &linha += '|'+trim(NfiCfopCod)                // N   4    CFOP
               &linha += '|'+trim(NfiPrdUndCod)                   // C   6    Unidade comercial
               &linha += '|'+trim(str(NfiQtd,15,4))         // N  12 4  Quantidade comercial
               &linha += '|'+trim(str(NfiVlrUnt,21,10))      // N  16 4  Valor unitario de  comercializacao
               &linha += '|'+trim(str(&Valor,15,2))         // N  15 2  Valor total bruto

               If Not Null(&PrdcEanTrib)
                    &linha += '|'+trim(&PrdcEanTrib)         // C  14    (cEANTrib)  GTIN (antigo ean) 
               Else
                    &linha += '|SEM GTIN'
               EndIf

               &linha += '|'+trim(NfiPrdUndCod)                   // C   6    Unidade Tributavel
               &linha += '|'+trim(str(NfiQtd,15,4))         // N  12 4  Quant.  Tributavel 
               &linha += '|'+trim(str(NfiVlrUnt,21,10))      // N  16 4  Valor unitario Tributavel

               if NfiVlrFrete > 0
                 &linha += '|'+trim(str(NfiVlrFrete,15,2))      // N  16 4  Valor Total do Frete
               else 
                 &linha += '|'
               endif

               &linha+='|' // Valor Total do Seguro
               
               if NfiVlrDsc > 0
                    &linha += '|'+ trim(str(NfiVlrDsc,15,2))      // N  15 2  Valor desconto
                    &desconto += NfiVlrDsc
               else
                    &linha+='|'
               endif

               if NfiOutDsp = 0
                   &linha += '|'                                  // N 6      outras despesas
               else
                   &linha += '|'+trim(str(NfiOutDsp,10,2))
               endif

               &linha += '|'+trim(str(NfiIndTot))             // N  1     Indica se valor do item entra no valor total
               &linha += '|' + trim(NfiOrdCompra)            // C  15      (xPed)           Número do Pedido de Compra
               &linha += '|' + Trim(NfiOrdCompraSeq)              // N   6      (nItemPed)       Item do Pedido de Compra
               &Linha += '|'+trim(NfiFCI)                     // C36      Numero do FCI
               &Linha += '|'                                  // C6       CODIGO NVE
                              
               do'cest'
               if not null(&CestCod)
                  &Linha += '|'+&CestCod                  // C7       CODIGO CEST
               else
                  &Linha += '|'                               // C7       CODIGO CEST
               endif

               If not Null(NfiPrdIndEscala)
                  &Linha += '|'+NfiPrdIndEscala.Trim()             // C1       Indicador de Escala Relevante
               Else
                  &Linha += '|S'
               EndIf

               if NfiPrdIndEscala = 'N'
                  &Linha += '|'+&EmpCnpj                    // C14  CNPJ do Fabricante
               else
                  &Linha += '|'
               endif

               &Linha += '|'+NfiPrdBenefFiscal.Trim()               // Código de Benefício Fiscal da UF
               &linha += '|' + &PrdCodBarras                        // C  30      (cBarra)         Código de Barras
               &linha += '|' + &PrdCodBarras                        // C  30      (cBarraTrib)     Código de Barras Tributável

               do 'tira_car'
    
               &NfiVlrPis = NfiVlrPis
               &NfiVlrCOF = NfiVlrCof

               &TotImp = NfiTotTribFedNac + NfiTotTribFedImp + NfiTotTribEst + NfiTotTribMun

              do case              
              case NfiCst = '10'
                    // CST - 10 - Tributa e com cobrança do icms por substituicao tributaria
                    if not null(&TotImp)
                        &linha = 'M|'+trim(str(&TotImp,10,2))+'|'
                    else
                        &linha = 'M|'
                    endif

                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()
                    
                    &linha = 'N|'
                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()
                    
                    &PerRedBc = (100 - &CfopBseIcmsSt)

                    &linha = 'N03|'+trim(str(NfiPrdOriPro,1))   //   N    1       Origem da Mercadoria
                    &linha += '|'+trim(NfiCst)                  //   N    2       Tributacao do Icms no sistema esta N 1       
                    &linha += '|'+trim(str(&CfopModBseIcms))    //   N    1       Modalidade de determinação da BC do ICMS
                    &linha += '|'+trim(str(NfiBseClcIcms,15,2)) //   N    15  2   Valor do BC do ICMS
                    &linha += '|'+trim(str(NfiAlqIcms,5,2))     //   N     5  2   Aliquota do imposto
                    &linha += '|'+trim(str(NfiVlrIcms,15,2))    //   N    15  2   Valor do ICMS
                    &linha += '|'+trim(str(&CfopModBseIcmsSt))  //   N     1      Modalidade de determinação da BC do ICMS ST
                    &linha += '|' +trim(str(&PerSt,6,2))        //   N     5  2   Percentual da margem de valor Adicionado do ICMS ST

                    if &PerRedBc > 0
                        &linha += '|'+trim(str(&PerRedBc,6,2))  //   N     5  2   Percentual da Redução de BC do ICMS ST
                    else 
                        &linha += '|'                             
                    endif
                    &linha += '|'+trim(str(NfiBseClcSt,15,2))   //   N    15  2   Valor da BC do ICMS ST
                    &linha += '|'+trim(str(NfiAlqSt,5,2))       //   N     5  2   Aliquota do imposto do ICMS ST
                    &linha += '|'+trim(str(NfiVlrSt,15,2))      //   N    15  2   Valor do ICMS ST
                    &linha += '|'                                           // N  13  2   (vBCFCP)         Valor da Base de calculo do Fundo de Combate a pobreza
                    &linha += '|'                                           // N   3  2   (pFCP)           Percentual do Fundo de Combate a Pobreza
                    &linha += '|'                                           // N  13  2   (vFCP)           Valor do Fundo de Combate a Pobreza
                    &linha += '|' + trim(str(NfiVlrBseClcFCPSub,15,2))      // N  13  2   (vBCFCPST)       Valor da Base de Calculo do FCP retido por substituição tributária
                    &linha += '|' + trim(str(NfiPercFCPSub,5,2))            // N   3  2   (pFCPST)         Percentual do Fundo de Combate a Pobreza retido por Substituição
                    &linha += '|' + trim(str(NfiVlrFCPSub,15,2))            // N  13  2   (VFCPST)         Valor do Fundo de Combate a Pobreza Retido por Substituição Tributária
                    &linha += '|'                                           // N  13  2   (vICMSSTDeson)   Valor do ICMS ST Desonerado
                    &linha += '|'                                           // N  13  2   (MotDesICMSST)   Motivo da Desoneração do ICMS

             case NfiCst = '20'

                    // CST - 20 - Com reduçao de base de calculo
                    if not null(&TotImp)
                        &linha = 'M|'+trim(str(&TotImp,10,2))+'|'
                    else
                        &linha = 'M|'
                    endif

                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha = 'N|'
                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()
                    
                    &PerRedBc = (100 - &CfopBseIcms) //&CfopBseIcmsSt

                    &linha = 'N04|'+trim(str(NfiPrdOriPro,1))        //   N     1      Origem da Mercadoria
                    &linha += '|'+trim(NfiCst)                       //   N     2      Tributacao do Icms         
                    &linha += '|'+trim(str(&CfopModBseIcms))         //   N     1      Modalidade de determinação da BC do ICMS)

                    if &PerRedBc > 0
                     &linha += '|'+trim(str(&PerRedBc,6,2))          //   N     5  2   Percentual da redução de BC   NfiPerRedBc
                    else
                     &linha += '|0.00'
                    endif

                    &linha += '|'+trim(str(NfiBseClcIcms,15,2))      //   N    15  2   Valor do BC do ICMS
                    &linha += '|'+trim(str(NfiAlqIcms,5,2))          //   N     5  2   Aliquota do imposto
                    &linha += '|'+trim(str(NfiVlrIcms,15,2))         //   N    15  2   Valor do ICMS
                    &linha += '|'+trim(str(NfiIcmsDes,10,2))         //   N    15 2   Valor do ICMS desonerado
                    &linha += '|'+trim(&CfopMotDesoneracao)          //   N    1      Motivo desoneração 
                    &linha += '|'                                    //   N   13  2   (vBCFCP)         Valor da Base de Calculo do Fundo de Combate a pobreza
                    &linha += '|'                                    //   N    3  2   (pFCP)           Percentual do Fundo de Combate a Pobreza
                    &linha += '|'                                    //   N   13  2   (vFCP)           Valor do Fundo de Combate a Pobreza

            case NfiCst = '30'

                    // CST - 30 - Isenta ou não tributada e c/ cobrança ICMS por S.T'
                    if not null(&TotImp)
                        &linha = 'M|'+trim(str(&TotImp,10,2))+'|'
                    else
                        &linha = 'M|'
                    endif

                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &PerRedBc = (100 - &CfopBseIcmsSt)

                    &linha = 'N|'
                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha = 'N05|'+trim(str(NfiPrdOriPro,1))           //   N    1      Origem da Mercadoria
                    &linha += '|'+trim(NfiCst)                          //   N    2      Tributacao do Icms        
                    &linha += '|'+trim(str(&CfopModBseIcmsSt))          //   N    1      Modalidade de determinação da BC do ICMS CST
                    &linha += '|'                                       //   N    5  2   Percentual da Margem de valor Adicionado do ICMS ST

                    if &PerRedBc > 0
                        &linha += '|'+trim(str(&PerRedBc,6,2))              //   N     5  2   Percentual da Redução de BC do ICMS ST
                    else 
                        &linha += '|0.00'                            
                    endif 

                    &linha += '|'+trim(str(NfiBseClcSt,15,2))           //   N   15  2   Valor da BC do ICMS ST
                    &linha += '|'+trim(str(NfiAlqSt,5))                 //   N    5  2   Aliquota do imposto do ICMS ST
                    &linha += '|'+trim(str(NfiVlrSt,15,2))              //   N   15  2   Valor do ICMS ST 
                    &linha += '|'+trim(str(NfiIcmsDes,10,2))            //   N    15 2   Valor do ICMS desonerado
                    &linha += '|'+trim(&CfopMotDesoneracao)             //   N    1      Motivo desoneração 
                    &linha += '|' + trim(str(NfiVlrBseClcFCPSub,15,2))  //   N  13  2   (vBCFCPST)       Valor da Base de Calculo do FCP retido por substituição tributária
                    &linha += '|' + trim(str(NfiPercFCPSub,5,2))        //   N   3  2   (pFCPST)         Percentual do Fundo de Combate a Pobreza retido por Substituição
                    &linha += '|' + trim(str(NfiVlrFCPSub,15,2))        //   N  13  2   (VFCPST)         Valor do Fundo de Combate a Pobreza Retido por Substituição Tributária

             case NfiCst = '40' // CST - 40 - Isenta, 41-Nao Tributada e 50- Suspensao

                    if not null(&TotImp)
                        &linha = 'M|'+trim(str(&TotImp,10,2))+'|'
                    else
                        &linha = 'M|'
                    endif

                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext() 

                    &linha = 'N|'
                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha = 'N06|'+trim(str(NfiPrdOriPro,1))              //   N    1      Origem da Mercadoria
                    &linha += '|'+trim(NfiCst)                             //   N    2      Tributacao do Icms 
                    &linha += '|'+trim(str(NfiIcmsDes,10,2))               //   N    15 2   Valor do ICMS desonerado
                    &linha += '|'+trim(&CfopMotDesoneracao)                //   N    1      Motivo desoneração 
  
             case NfiCst = '41' 

                    if not null(&TotImp)
                        &linha = 'M|'+trim(str(&TotImp,10,2))+'|'
                    else
                        &linha = 'M|'
                    endif

                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha = 'N|'
                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha = 'N06|'+trim(str(NfiPrdOriPro,1))              //   N    1      Origem da Mercadoria
                    &linha += '|'+trim(NfiCst)                             //   N    2      Tributacao do Icms  
                    &linha += '|'+trim(str(NfiIcmsDes,10,2))               //   N    15 2   Valor do ICMS desonerado
                    &linha += '|'+trim(&CfopMotDesoneracao)                //   N    1      Motivo desoneração 

             case NfiCst = '50'
                    
                    if not null(&TotImp)
                        &linha = 'M|'+trim(str(&TotImp,10,2))+'|'
                    else
                        &linha = 'M|'
                    endif

                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext() 

                    &linha = 'N|'
                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha = 'N06|'+trim(str(NfiPrdOriPro,1))              //   N    1      Origem da Mercadoria
                    &linha += '|'+trim(NfiCst)                             //   N    2      Tributacao do Icms  
                    &linha += '|'+trim(str(NfiIcmsDes,10,2))               //   N    15 2   Valor do ICMS desonerado
                    &linha += '|'+trim(&CfopMotDesoneracao)                //   N    1      Motivo desoneração  
                         
             case NfiCst = '51'
                    // CST - 51- Diferimento
                    if not null(&TotImp)
                        &linha = 'M|'+trim(str(&TotImp,10,2))+'|'
                    else
                        &linha = 'M|'
                    endif

                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha = 'N|'
                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &PerRedBc = (100 - &CfopBseIcms)

                    &linha = 'N07|'+trim(str(NfiPrdOriPro,1))  //   N     1      Origem da Mercadoria
                    &linha += '|'+trim(NfiCst)                 //   N     2      Tributacao do Icms         
                    &linha += '|'+trim(str(&CfopModBseIcms))   //   N    1      Modalidade de determinação da BC do ICMS

                    if NfiPerRedBc > 0
                        &linha += '|'+trim(str(NfiPerRedBc,6,2))    //   N     5  2   Percentual da Redução de BC do ICMS
                    else 
                        &linha += '|0.00'                           //   N     5  2   Percentual da Redução de BC do ICMS
                    endif

                    if NfiBseClcIcmsInt > 0
                       &linha += '|'+trim(str(NfiBseClcIcms,15,2))   //   N     15 2 Valor da BC do ICMS 
                    else
                       &linha += '|0.00'
                    endif

                    if NfiAlqIcms > 0
                       &linha += '|'+trim(str(NfiAlqIcms,15,2))         //   N     5 2 Aliquota do Imposto
                    else
                       &linha += '|0.00'
                    endif

                    if NfiVlrIcms > 0
                       &linha += '|'+trim(str(NfiVlrIcms,15,2))         //   N     15 2 Valor do ICMS 
                    else
                       &linha += '|0.00'
                    endif

                    if NfiVlrIcmsDiferimento > 0
                       &linha += '|'+trim(str(NfiVlrIcmsDiferimento,15,2))     //   N     15 2 Valor do ICMS da Operação
                    else
                       &linha += '|0.00'
                    endif

                    if NfiPerDiferimento > 0
                       &linha += '|'+trim(str(NfiPerDiferimento,15,2))         //   N     15  2 % do Diferimento
                    else
                       &linha += '|100.00'
                    endif

                    if NfiVlrDiferimento > 0
                       &linha += '|'+trim(str(NfiVlrDiferimento,15,2))         //   N     15  2 valor do ICMS Diferido
                    else
                       &linha += '|0.00'
                    endif

                    &linha += '|'                                    // N  13  2   (vBCFCP)         Valor da Base de Calculo do Fundo de Combate a pobreza
                    &linha += '|'                                    // N   3  2   (pFCP)           Percentual do Fundo de Combate a Pobreza
                    &linha += '|'                                    // N  13  2   (vFCP)           Valor do Fundo de Combate a Pobreza
                    &linha += '|'                                    // N  13  2   (pFCPDif)        Percentual do Diferimento referente ao FCP
                    &linha += '|'                                    // N  13  2   (vFCPDif)        Valor do Diferimento referente ao FCP
                    &linha += '|'                                    // N  13  2   (vFCPEfet)       Valor Efetivo do ICMS Referente ao FCP                    

             case NfiCst = '60'
                    // CST - 60- Icms cobrado por substituicao tributaria
                    if not null(&TotImp)
                        &linha = 'M|'+trim(str(&TotImp,10,2))+'|'
                    else
                        &linha = 'M|'
                    endif

                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha = 'N|'
                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha  = 'N08|'+trim(str(NfiPrdOriPro,1))          // N       1      Origem da Mercadoria
                    &linha += '|'+trim(NfiCst)                          // N       2      Tributacao do Icms                           
                    &linha += '|'+trim(str(NfiBseClcSt060,15,2))        // N      15  2 (vBCSTRet)       Valor do BC do ICMS ST
                    &linha += '|'+trim(str(NfiVlrSt060 ,15,2))          // N      15  2 (vICMSSTRet)     Valor do ICMS ST
                    &linha += '|'+trim(str(NfiAliqSt060 ,6,2))          // N       3  2 (pST)            Alíquota suportada pelo Consumidor Final
                    &linha += '|'+trim(str(NfiVlrBseClcFcpStRet ,15,2)) // N      13  2 (vBCFCPSTRet)    Valor da Base de Cálculo do FCP retido anteriormente por ST
                    &linha += '|'+trim(str(NfiPercFcpStRet ,6,2))       // N       3  2 (pFCPSTRet)      Valor da Base de Cálculo do FCP retido anteriormente por ST
                    &linha += '|'+trim(str(NfiVlrFcpStRet ,15,2))       // N      13  2 (vFCPSTRet)      Valor do FCP retido por Substituição Tributária
                    &linha += '|'+trim(str(NfiPerRedBc ,15,2))          // N      13  2 (pRedBCEfet)     Percentual de Redução da BC Efetiva
                    &linha += '|'+trim(str(NfiVlrBseClcIcmsEfet ,15,2)) // N      13  2 (vBCEfet)        Valor da base de cálculo efetiva
                    &linha += '|'+trim(str(NfiPercIcmsEfet ,6,2) )      // N       3  2 (pICMSEfet)      Alíquota do ICMS efetiva
                    &linha += '|'+trim(str(NfiVlrIcmsEfet ,15,2))       // N      13  2 (vICMSEfet)      Valor do ICMS efetivo
                    &linha += '|'+trim(str(NfiVlrIcmsSubstituto ,15,2)) // N      13  2 (vICMSSubstituto)Valor do ICMS próprio do Substituto
    
             case NfiCst = '70'
                      // CST - 70 - Com reduçao de base de calculo e cobrança do icms por subs. tributaria
                    
                    if not null(&TotImp)
                        &linha = 'M|'+trim(str(&TotImp,10,2))+'|'
                    else
                        &linha = 'M|'
                    endif

                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha = 'N|'
                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()
                    
                    &PerRedBc = (100 - &CfopBseIcms)

                    &linha  = 'N09|'+trim(str(NfiPrdOriPro,1))              //origem da mercadoria 
                    &linha += '|'+trim(NfiCst)                              //   N    2      Tributacao do Icms 
                    &linha += '|'+trim(str(&CfopModBseIcms))                //   N    1      Modalidade de determinação da BC do ICMS 

                    if &PerRedBc > 0
                    &linha += '|'+trim(str(&PerRedBc,6,2))                  //   N     5  2    Percentual de reduçao da bc
                     else 
                        &linha += '|0.00'                             
                    endif  

                    &linha += '|'+trim(str(NfiBseClcIcms,15,2))             //   N   15  2   Valor da BC do ICMS 
                    &linha += '|'+trim(str(NfiAlqIcms,5,2))                 //   N    5  2   Aliquota do imposto  
                    &linha += '|'+trim(str(NfiVlrIcms,15,2))                //   N   15  2   Valor do ICMS         
                    &linha += '|'+trim(str(&CfopModBseIcmsSt))              //   N    1      Modalidade de determinação da BC do ICMS ST
                    &linha += '|'+TRIM(str(&PerSt,6,2))                     //   N    5  2   Percentual da Margem de valor Adicionado do ICMS ST(MVA)

                    &PerRedBc = (100 - &CfopBseIcmsST)

                    if &PerRedBc > 0
                    &linha += '|'+trim(str(&PerRedBc,6,2))                  //   N     5  2    Percentual da Redução de BC do ICMS ST
                     else 
                        &linha += '|0.00'                              
                    endif  

                    &linha += '|'+trim(str(NfiBseClcSt,15,2))               //   N   15  2   Valor da C do ICMS ST
                    &linha += '|'+trim(str(NfiAlqSt,5,2))                   //   N    5  2   Aliquota do imposto do ICMS ST  
                    &linha += '|'+trim(str(NfiVlrSt,15,2))                  //   N   15  2   Valor do ICMS ST 
                    &linha += '|'+trim(str(NfiIcmsDes,10,2))                //   N    15 2   Valor do ICMS desonerado
                    &linha += '|'+trim(&CfopMotDesoneracao)                 //   N    1      Motivo desoneração  
                    &linha += '|'                                           //   N  13  2   (vBCFCP)         Valor da Base de calculo do Fundo de Combate a pobreza
                    &linha += '|'                                           //   N   3  2   (pFCP)           Percentual do Fundo de Combate a Pobreza
                    &linha += '|'                                           //   N  13  2   (vFCP)           Valor do Fundo de Combate a Pobreza
                    &linha += '|' + trim(str(NfiVlrBseClcFCPSub,15,2))      //   N  13  2   (vBCFCPST)       Valor da Base de Calculo do FCP retido por substituição tributária
                    &linha += '|' + trim(str(NfiPercFCPSub,5,2))            //   N   3  2   (pFCPST)         Percentual do Fundo de Combate a Pobreza retido por Substituição
                    &linha += '|' + trim(str(NfiVlrFCPSub,15,2))            //   N  13  2   (VFCPST)         Valor do Fundo de Combate a Pobreza Retido por Substituição Tributária
                    &linha += '|'                                           //   N   3  2   (vICMSSTDeson)   Valor do ICMS ST Desonerado
                    &linha += '|'                                           //   N  13  2   (MotDesICMSST)   Motivo da Desoneração do ICMS

            case NfiCst = '90'
                    // CST - 90 - Outros
                    if not null(&TotImp)
                        &linha = 'M|'+trim(str(&TotImp,10,2))+'|'
                    else
                        &linha = 'M|'
                    endif

                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                   // Icms 'N|'
                    &linha = 'N|'
                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &PerRedBc = ( 100 - &CfopBseIcms)

                    &linha  = 'N10|'+trim(str(NfiPrdOriPro,1))              //   N    1      Origem da Mercadoria
                    &linha += '|'+trim(NfiCst)                              //   N    2      Tributacao do Icms        
                    &linha += '|'+trim(str(&CfopModBseIcms))                //   N    1      Modalidade de determinação da BC do ICMS

                    if &PerRedBc > 0 
                        &linha += '|'+trim(str(&PerRedBc,6,2))              //   N    5  2   Percentual de reduçao da bc
                    else 
                        &linha += '|0.00'                                 
                    endif

                    &linha += '|'+trim(str(NfiBseClcIcms,15,2))             //   N   15  2   Valor da BC do ICMS                     
                    &linha += '|'+trim(str(NfiAlqIcms,5,2))                 //   N    5  2   Aliquota do imposto  
                    &linha += '|'+trim(str(NfiVlrIcms,15,2))                //   N   15  2   Valor do ICMS         
                    &linha += '|'+trim(str(&CfopModBseIcmsSt))              //   N    1      Modalidade de determinação da BC do ICMS ST                                        
                    &linha += '|'+TRIM(str(&PerSt,6,2))                     //   N    5  2   Percentual da Margem de valor Adicionado do ICMS ST

                    &PerRedBc = (100 - &CfopBseIcmsST)

                    if &PerRedBc > 0 
                        &linha += '|'+trim(str(&PerRedBc,5,2))              //   N    5  2   Percentual da Redução de BC do ICMS ST  
                    else 
                        &linha += '|0.00'                                 
                    endif
                   
                   &linha += '|'+trim(str(NfiBseClcSt,15,2))                //   N   15  2   Valor da C do ICMS ST
                   &linha += '|'+trim(str(NfiAlqSt,5,2))                    //   N    5  2   Aliquota do imposto do ICMS ST
                   &linha += '|'+trim(str(NfiVlrSt,15,2))                   //   N   15  2   Valor do ICMS ST  
                   &linha += '|'+trim(str(NfiIcmsDes,10,2))                 //   N    15 2   Valor do ICMS desonerado
                   &linha += '|'+trim(&CfopMotDesoneracao)                  //   N    1      Motivo desoneração                       
                   &linha += '|'                                            //   N  13  2   (vBCFCP)         Valor da Base de calculo do Fundo de Combate a pobreza
                   &linha += '|'                                            //   N   3  2   (pFCP)           Percentual do Fundo de Combate a Pobreza
                   &linha += '|'                                            //   N  13  2   (vFCP)           Valor do Fundo de Combate a Pobreza
                   &linha += '|' + trim(str(NfiVlrBseClcFCPSub,15,2))       //   N  13  2   (vBCFCPST)       Valor da Base de Calculo do FCP retido por substituição tributária
                   &linha += '|' + trim(str(NfiPercFCPSub,5,2))             //   N   3  2   (pFCPST)         Percentual do Fundo de Combate a Pobreza retido por Substituição
                   &linha += '|' + trim(str(NfiVlrFCPSub,15,2))             //   N  13  2   (VFCPST)         Valor do Fundo de Combate a Pobreza Retido por Substituição Tributária
                   &linha += '|'                                            //   N   3  2   (vICMSSTDeson)   Valor do ICMS ST Desonerado
                   &linha += '|'                                            //   N  13  2   (MotDesICMSST)   Motivo da Desoneração do ICMS

            case NfiCst = '00'
                    // CST - 00 - Tributada integralmente
                    if not null(&TotImp)
                        &linha = 'M|'+trim(str(&TotImp,10,2))+'|'
                    else
                        &linha = 'M|'
                    endif

                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()


                    &linha = 'N|'
                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha = 'N02|'+trim(str(NfiPrdOriPro,1))       //   N    1      Origem da Mercadoria
                    &linha += '|'+Padl(trim(NfiCst),2,'0')          //   N    2      Tributação do ICMS: 00 - Tributada Integralmente                    
                    &linha += '|'+trim(str(&CfopModBseIcms))        //   N    1      Modalidade de determinação da BC do ICMS                    
                    &linha += '|'+trim(str(NfiBseClcIcms,15,2))     //   N    15  2  Valor da BC do ICMS
                    &linha += '|'+trim(str(NfiAlqIcms,5,2))         //   N     5  2  Aliquota do imposto 
                    &linha += '|'+trim(str(NfiVlrIcms,15,2))        //   N    15  2  Valor do ICMS
                    &linha += '|'                                   //   N    3  2   (pFCP)           Percentual do Fundo de Combate a Pobreza
                    &linha += '|'                                   //   N   13  2   (vFCP)           Valor do Fundo de Combate a Pobreza
 
           case NfiCst ="101"       

                    if not null(&TotImp)
                        &linha = 'M|'+trim(str(&TotImp,10,2))+'|'
                    else
                        &linha = 'M|'
                    endif

                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha = 'N|'
                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha = 'N10c|'+trim(str(NfiPrdOriPro,1))                                    //   N    1      Origem da Mercadoria 
                    &linha += '|'+Padl(trim(NfiCst),3,'0')                            //   N    2      Tributação do ICMS: 101
                    &linha += '|'+trim(str(&EmpPerCredIcms,5,2))                    //   N    5  2   Aliquota aplicavel de caclulo de crédito                   
                    &linha += '|' + trim(str(NfiVlrCredIcms ,15,2))                 //   N   15 2   Valor crédito do ICMS que pode ser aproveitado                                                        
              
        
           case NfiCst ="102" or NfiCst ="103" or NfiCst ="300" or NfiCst ="400" // SEM NENHUMA CONTA


                    if not null(&TotImp)
                        &linha = 'M|'+trim(str(&TotImp,10,2))+'|'
                    else
                        &linha = 'M|'
                    endif

                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha = 'N|'
                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha = 'N10d|'+trim(str(NfiPrdOriPro,1))                             //   N    1      Origem da Mercadoria
                    &linha += '|'+Padl(trim(NfiCst),3,'0')                     //   N    2      Tributação do ICMS: 102,103,300,400


          case NfiCst = "201" // COM CALCULO D SUBS TRIBUTÁRIA 

                    if not null(&TotImp)
                        &linha = 'M|'+trim(str(&TotImp,10,2))+'|'
                    else
                        &linha = 'M|'
                    endif

                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha = 'N|'
                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha = 'N10e|'+trim(str(NfiPrdOriPro,1))          //   N    1      Origem da Mercadoria 
                    &linha += '|'+Padl(trim(NfiCst),3,'0')              //   N    2      Tributação do ICMS: 201
                    &linha += '|'+trim(str(&CfopModBseIcmsSt))          //   N    1      Modalidade de determinação da BC do ICMS ST
                    &linha += '|'+TRIM(str(&PerSt,6,2))                 //   N    5  2   Percentual da Margem de valor Adicionado do ICMS ST(MVA)
                    &linha += '|'                                       //   N    5  2   Percentual da Redução de BC do ICMS ST 
                    &linha += '|'+trim(str(NfiBseClcSt,15,2))           //   N    15 2   Valor da BC do ICMS ST
                    &linha += '|'+trim(str(NfiAlqSt,5,2))               //   N     5 2   Aliquota do imposto do ICMS ST
                    &linha += '|'+trim(str(NfiVlrSt,15,2))              //   N    15 2   Valor do ICMS ST
                    &linha += '|'+trim(str(&EmpPerCredIcms,5,2))        //   N    5  2   Aliquota aplicavel de caclulo de crédito                   
                    &linha += '|' + trim(str(NfiVlrCredIcms ,15,2))     //   N   15 2   Valor crédito do ICMS que pode ser aproveitado                                                        
                    &linha += '|' + trim(str(NfiVlrBseClcFCPSub,15,2))  //   N   13  2   (vBCFCPST)       Valor da Base de Calculo do FCP retido por substituição tributária
                    &linha += '|' + trim(str(NfiPercFCPSub,5,2))        //   N    3  2   (pFCPST)         Percentual do Fundo de Combate a Pobreza retido por Substituição
                    &linha += '|' + trim(str(NfiVlrFCPSub,15,2))        //   N   13  2   (VFCPST)         Valor do Fundo de Combate a Pobreza Retido por Substituição Tributária

                       
          case NfiCst ="202" or NfiCst ="203"  // COM CALCULO D SUBS TRIBUTÁRIA  
                        
                    if not null(&TotImp)
                        &linha = 'M|'+trim(str(&TotImp,10,2))+'|'
                    else
                        &linha = 'M|'
                    endif

                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext() 

                    &linha = 'N|'
                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha = 'N10f|'+trim(str(NfiPrdOriPro,1))          //   N    1      Origem da Mercadoria
                    &linha += '|'+Padl(trim(NfiCst),3,'0')              //   N    2      Tributação do ICMS: 202,203
                    &linha += '|'+trim(str(&CfopModBseIcmsSt))          //   N    1      Modalidade de determinação da BC do ICMS ST
                    &linha += '|'+TRIM(str(&PerSt,6,2))                 //   N    5  2   Percentual da Margem de valor Adicionado do ICMS ST(MVA)
                    &linha += '|'                                       //   N    5  2   Percentual da Redução de BC do ICMS ST 
                    &linha += '|'+trim(str(NfiBseClcSt,15,2))           //   N    15 2   Valor da BC do ICMS ST
                    &linha += '|'+trim(str(NfiAlqSt,5,2))               //   N     5 2   Aliquota do imposto do ICMS ST
                    &linha += '|'+trim(str(NfiVlrSt,15,2))              //   N    15 2   Valor do ICMS ST
                    &linha += '|' + trim(str(NfiVlrBseClcFCPSub,15,2))  //   N   13  2   (vBCFCPST)       Valor da Base de Calculo do FCP retido por substituição tributária
                    &linha += '|' + trim(str(NfiPercFCPSub,5,2))        //   N    3  2   (pFCPST)         Percentual do Fundo de Combate a Pobreza retido por Substituição
                    &linha += '|' + trim(str(NfiVlrFCPSub,15,2))        //   N   13  2   (VFCPST)         Valor do Fundo de Combate a Pobreza Retido por Substituição Tributária                   
                     
        
          case NfiCst ="500" // ST COBRADA ANTERIORMENTE

                    if not null(&TotImp)
                        &linha = 'M|'+trim(str(&TotImp,10,2))+'|'
                    else
                        &linha = 'M|'
                    endif

                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha = 'N|'
                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha = 'N10g|'+trim(str(NfiPrdOriPro,1))              // N     1     Origem da Mercadoria 
                    &linha += '|'+Padl(trim(NfiCst),3,'0')                  // N     2     Tributação do ICMS: 500
                    &linha += '|'+trim(str(NfiBseClcSt060,15,2))        // N      15  2 (vBCSTRet)       Valor do BC do ICMS ST
                    &linha += '|'+trim(str(NfiVlrSt060 ,15,2))          // N      15  2 (vICMSSTRet)     Valor do ICMS ST
                    &linha += '|'+trim(str(NfiAliqSt060 ,6,2))          // N       3  2 (pST)            Alíquota suportada pelo Consumidor Final
                    &linha += '|'+trim(str(NfiVlrBseClcFcpStRet ,15,2)) // N      13  2 (vBCFCPSTRet)    Valor da Base de Cálculo do FCP retido anteriormente por ST
                    &linha += '|'+trim(str(NfiPercFcpStRet ,6,2))       // N       3  2 (pFCPSTRet)      Valor da Base de Cálculo do FCP retido anteriormente por ST
                    &linha += '|'+trim(str(NfiVlrFcpStRet ,15,2))       // N      13  2 (vFCPSTRet)      Valor do FCP retido por Substituição Tributária
                    &linha += '|'+trim(str(NfiPerRedBc ,15,2))          // N      13  2 (pRedBCEfet)     Percentual de Redução da BC Efetiva
                    &linha += '|'+trim(str(NfiVlrBseClcIcmsEfet ,15,2)) // N      13  2 (vBCEfet)        Valor da base de cálculo efetiva
                    &linha += '|'+trim(str(NfiPercIcmsEfet ,6,2) )      // N       3  2 (pICMSEfet)      Alíquota do ICMS efetiva
                    &linha += '|'+trim(str(NfiVlrIcmsEfet ,15,2))       // N      13  2 (vICMSEfet)      Valor do ICMS efetivo
                    &linha += '|'+trim(str(NfiVlrIcmsSubstituto ,15,2)) // N      13  2 (vICMSSubstituto)Valor do ICMS próprio do Substituto
 
        
          case NfiCst="900" // VARIOS CAMPO - DE PERC DE CREDITO - ST
                        
                    if not null(&TotImp)
                        &linha = 'M|'+trim(str(&TotImp,10,2))+'|'
                    else
                        &linha = 'M|'
                    endif

                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &linha = 'N|'
                    &ret = DFWPTxt(&Linha,1000)
                    &ret = DFWNext()

                    &PerRedBc = ( 100 - &CfopBseIcms)

                    &linha = 'N10h|'+trim(str(NfiPrdOriPro,1))         //   N    1      Origem da Mercadoria  
                    &linha += '|'+Padl(trim(NfiCst),3,'0')             //   N    2      Tributação do ICMS: 900
                    &linha += '|'+trim(str(&CfopModBseIcms))           //   N    1      Modalidade de determinação da BC do ICMS                    
                    &linha += '|'+trim(str(NfiBseClcIcms,15,2))        //   N   15  2   Valor da BC do ICMS ST

                    if &PerRedBc > 0
                        &linha += '|'+trim(str(&PerRedBc,6,2))         //   N    5  2   Percentual da Redução de BC do ICMS  
                    else 
                        &linha += '|0.00'                                                      
                    endif  

                    &linha += '|'+trim(str(NfiAlqIcms ,5,2))           //   N    5  2   Aliquota do imposto do ICMS
                    &linha += '|'+trim(str(NfiVlrIcms,15,2))           //   N   15  2   Valor do ICMS      
                    &linha += '|'+trim(str(&CfopModBseIcmsSt))         //   N    1      Modalidade de determinação da BC do ICMS ST
                    
                    if &PerSt > 0
                         &linha += '|'+TRIM(str(&PerSt,6,2))           //   N    5  2   Percentual da Margem de valor Adicionado do ICMS ST(MVA) 
                    else
                         &linha += '|0.00'                                                          
                    endif

                    &PerRedBc = ( 100 - &CfopBseIcmsST)
                    
                    if &PerRedBc > 0
                        &linha += '|'+trim(str(&PerRedBc,6,2))         //   N    5  2   Percentual da Redução de BC do ICMS ST 
                    else 
                        &linha += '|0.00'  
                    endif  

                   &linha += '|'+trim(str(NfiBseClcSt,15,2))           //   N    15 2   Valor da BC do ICMS ST
                   &linha += '|'+trim(str(NfiAlqSt,5,2))               //   N     5 2   Aliquota do imposto do ICMS ST
                   &linha += '|'+trim(str(NfiVlrSt,15,2))              //   N    15 2   Valor do ICMS ST
                   &linha += '|'+trim(str(&EmpPerCredIcms,5,2))        //   N    5  2   Aliquota aplicavel de caclulo de crédito                   
                   &linha += '|' + trim(str(NfiVlrCredIcms ,15,2))     //   N   15 2   Valor crédito do ICMS que pode ser aproveitado                                                        
                   &linha += '|' + trim(str(NfiVlrBseClcFCPSub,15,2))  //   N   13  2   (vBCFCPST)       Valor da Base de Calculo do FCP retido por substituição tributária
                   &linha += '|' + trim(str(NfiPercFCPSub,5,2))        //   N    3  2   (pFCPST)         Percentual do Fundo de Combate a Pobreza retido por Substituição
                   &linha += '|' + trim(str(NfiVlrFCPSub,15,2))        //   N   13  2   (VFCPST)         Valor do Fundo de Combate a Pobreza Retido por Substituição Tributária                   

            endcase
            do 'tira_car'

            if NfiVlrBseIcmsDest > 0
            
                &Linha = 'NA01|'+trim(str(NfiVlrBseIcmsDest,10,2))              // N    15  2  (vBCUFDest)     Valor da Base de calculo de Icms na UF destino 
                &Linha += '|'+trim(str(NfiPerFCP,10,2))                         // N     6  2  (pFCPUFDest)    Percentual do ICMS relativo ao Fundo de Cpmbate a Pobreza (FCP) na UF de destino
                &Linha += '|'+trim(str(NfiAlqInter,10,2))                       // N     6  2  (pICMSUFDest)   Aliquota interna da UF de destino
                &Linha += '|'+trim(str(NfiAlqIcms,10,2))                        // N     6  2  (pICMSInter)    Aliquota interestadual das UF envolvidas
    
                do case                                                         // N     6  2  (pICMSInterPart)Percentual provisorio de partilha do ICMS Interestadual
                   case year(&Today) = 2015 or year(&Today) = 15                
                        &Linha += '|40'
                   case year(&Today) = 2016 or year(&Today) = 16 
                        &Linha += '|40'
                   case year(&Today) = 2017 or year(&Today) = 17
                        &Linha += '|60'
                   case year(&Today) = 2018 or year(&Today) = 18
                        &Linha += '|80'
                   OtherWise
                        &Linha += '|100'
                endcase
    
                &Linha += '|'+trim(str(NfiVlrFCP,10,2))                         // N    15  2  (vFCPUFDest)    Valor do ICMS relativo ao Fundo de Combate a Pobreza (FCP) da UF de destino
                &Linha += '|'+trim(str(NfiVlrIcmsDest,10,2))                    // N    15  2  (vICMSUFDest)   Valor do icms interestadual para a UF de destino           
                &Linha += '|'+trim(str(NfiVlrIcmsOri,10,2))                     // N    15  2  (vICMSUFRemet)  Valor do icms interestadual para a UF do remetente

                If NfiVlrFCP > 0
                    &linha += '|'   + trim(str(NfiVlrBseIcmsDest,15,2))         // N    13  2  (vBCFUFDest)    Valor da Base de Cálculo do Fundo de Combate a Pobreza na UF de destino
                else
                    &linha += '|0.00'
                endif                
                
                do 'tira_car'
            
            endif

            // IPI 
            If NfiCstIpi =  '00' or NfiCstIpi = '49' or NfiCstIpi = '50' or NfiCstIpi = '99' 
                    
                  &linha = 'O|'

                  if null(&CfopEnqCod)
                     &linha += '||||999'
                  else
                     &linha += '||||'+&CfopEnqCod
                  endif

                  do 'tira_car'
                  &linha = 'O07|'+trim(NfiCstIpi)              //   C    2       Cod. situacao trib. do IPI
                  &linha += '|'+trim(str(NfiVlrIpi,15,2))           //   N   15  2    Valor do IPI
                  do 'tira_car'

                  If NfiVlrIpi > 0
                     // &linha = 'O10|'+trim(str(NfiTotPrd,15,2))   //   N   15  2    Valor da base de calc. do IPI // Alterado dia 21/12/2017: Base do IPI está sem o desconto, frete, acréscimo
                     &linha = 'O10|'+trim(str(NfiBseClcIPI ,15,2))   //   N   15  2    Valor da base de calc. do IPI
                  else
                     &linha = 'O10|0.00'                             //   N   15  2    Valor da base de calc. do IPI
                  endif

                  &linha += '|'+trim(str(NfiAlqIpi,15,2))           //   N    5  2    Aliquota do IPI
                  do 'tira_car'
                     
            else
               
                  &linha = 'O|'

                  if null(&CfopEnqCod)
                     &linha += '||||999'
                  else
                     &linha += '||||'+&CfopEnqCod
                  endif

                  do 'tira_car'
                  &linha = 'O08|'+trim(NfiCstIpi)                //   C     2      Cod. situacao trib. do IPI  
                  do 'tira_car'

            endif
                        
               
            // PIS - GRUPO de Pis tributado pela aliquota
            do case
               case NfiCstPis = '01' or NfiCstPis = '02'
                  
                  &linha = 'Q|'         
                  &ret = DFWPTxt(&Linha,1000)
                  &ret = DFWNext() 

                  &linha = 'Q02|'+NfiCstPis                //   N      2         Codigo da Situação Tributaria do PIS       

                  if NfiAlqPis > 0
                     // &linha += '|'+trim(str(NfiTotPrd,15,2))   //   N     15 2       Valor da Base de Calculo do PIS // Alterado dia 21/12/2017: Base do PIS está sem o desconto, frete, acréscimo
                     &linha += '|'+trim(str(NfiBseClcPIS ,15,2))   //   N     15 2       Valor da Base de Calculo do PIS
                     &linha += '|'+trim(str(NfiAlqPis,5,2))         //   N      5 2       Aliquota do PIS(em percentual)
                     &linha += '|'+trim(str(NfiVlrPis,15,2))         //   N     15 2       Valor do PIS
                     do 'tira_car'
                  else
                     &linha += '|0.00'                              //   N     15 2       Valor da Base de Calculo do PIS
                     &linha += '|0.00'                               //   N      5 2       Aliquota do PIS(em percentual)
                     &linha += '|0.00'                                //   N     15 2       Valor do PIS
                     do 'tira_car'
                  endif

               // Grupo de Pis tributado por quantidade
               case NfiCstPis = '03'
                  
                  &linha = 'Q|'         
                  &ret = DFWPTxt(&Linha,1000)
                  &ret = DFWNext() 
                  &linha = 'Q03|'+'03'                            //   N      2         Codigo da Situação Tributaria do PIS
                  &linha += '|'+trim(str(NfiQtd,16,4))            //   N     16 4       Quatidade Vendida   
                  &linha += '|0.00'
                  &linha += '|'+trim(str(NfiVlrPis,15,2))         //   N     15 2       Valor do PIS   
                  do 'tira_car'

               // Grupo de Pis nao tributado 
               case NfiCstPis = '04' or NfiCstPis = '06' or NfiCstPis = '07' or NfiCstPis = '08' or NfiCstPis = '09'
                  &linha = 'Q|'         
                  &ret = DFWPTxt(&Linha,1000)
                  &ret = DFWNext()
                  &linha = 'Q04|'+NfiCstPis                 //   N      2         Codigo da Situação Tributaria do PIS
                  &ret = DFWPTxt(&Linha,1000)
                  &ret = DFWNext()

               // Grupo de Pis outras operacoes
               otherwise

                   &linha = 'Q|'         
                   &ret = DFWPTxt(&Linha,1000)
                   &ret = DFWNext()
                   &linha = 'Q05|'+NfiCstPis                         //   N      2         Codigo da Situação Tributaria do PIS
                   &linha += '|'+trim(str(NfiVlrPis,15,2))         //   N      15 2      Valor do PIS                   
                   do 'tira_car'
                  

                  if NfiAlqPis > 0
                     // &linha = 'Q07|'+trim(str(NfiTotPrd,15,2))   //   N     15 2       Valor da Base de Calculo do PIS // Alterado dia 21/12/2017: Base do PIS está sem o desconto, frete, acréscimo
                     &linha = 'Q07|'+trim(str(NfiBseClcPIS ,15,2))   //   N     15 2       Valor da Base de Calculo do PIS
                     &linha += '|'+trim(str(NfiAlqPis,5,2))          //   N     5 2       Aliquota do PIS(percentual)  
                     do 'tira_car'
                  else
                     &linha = 'Q07|0.00'                               //   N     15 2       Valor da Base de Calculo do PIS
                     &linha += '|0.00'                                //   N     5 2       Aliquota do PIS(percentual)  
                     do 'tira_car'
                  endif
                                         
            endcase
              

            // Grupo de COFINS tributado pela aliquota
            do case
                case NfiCstCof = '01' or NfiCstCof = '02'
                   &linha = 'S|'
                   &ret = DFWPTxt(&Linha,1000)
                   &ret = DFWNext()
    
                   &linha = 'S02|'+NfiCstCof.Trim()                  //   N     2           Codigo de Situação Tributaria do COFINS     
    
                   if NfiAlqCof > 0
                      // &linha += '|'+trim(str(NfiTotPrd,15,2))    //   N     15  2       Valor da Base de Calculo da COFINS // Alterado dia 21/12/2017: Base do COFINS está sem o desconto, frete, acréscimo
                      &linha += '|'+trim(str(NfiBseClcCOFINS ,15,2))    //   N     15  2       Valor da Base de Calculo da COFINS
                      &linha += '|'+trim(str(NfiAlqCof,5,2))          //   N     5   2       Aliquota da COFINS(Percentual)
                      &linha += '|'+trim(str(NfiVlrCof,15,2))          //   N     15  2       Valor do COFINS
                      do 'tira_car'
                   else
                      &linha += '|0.00'                                //   N     15  2       Valor da Base de Calculo da COFINS
                      &linha += '|0.00'                                //   N     5   2       Aliquota da COFINS(Percentual)
                      &linha += '|0.00'                                //   N     15  2       Valor do COFINS
                      do 'tira_car'
                   endif
               
 
                 // Grupo de Cofins tributado por QTDE
                 case NfiCstCof = '03'
                       &linha = 'S|'
                       &ret = DFWPTxt(&Linha,1000)
                       &ret = DFWNext()             
                       &linha = 'S03|'+NfiCstCof.Trim()                           //   N     2           Codigo de Situação Tributaria do COFINS     
                       &linha += '|'+trim(str(NfiQtd,16,4))            //   N     16  4       Quantidade vendida
                       &linha += '|0.00'                    //   N     15  4       Aliquota do COFINS(Reais)
                       &linha += '|'+trim(str(NfiVlrCof,15,2))         //   N     15  2       Valor do COFINS
                       do 'tira_car'
                      
                  // Grupo de Cofins nao tributado 
                  case NfiCstCof = '04' or NfiCstCof = '06' or NfiCstCof = '07' or NfiCstCof = '08' or NfiCstCof = '09'
                       &linha = 'S|'
                       &ret = DFWPTxt(&Linha,1000)
                       &ret = DFWNext()   
                       &linha = 'S04|'+NfiCstCof                //   N     2           Codigo de Situação Tributaria do COFINS     
                       do 'tira_car'
                      
                  // Grupo de Cofins outras operacoes
                  otherwise
        
                       &linha = 'S|'
                       &ret = DFWPTxt(&Linha,1000)
                       &ret = DFWNext()   
         
                       &linha = 'S05|'+NfiCstCof                      //   N     2           Codigo de Situação Tributaria do COFINS     
                       &linha += '|'+trim(str(NfiVlrCof,15,2))    //   N     15  2       Valor do COFINS 
                       do 'tira_car'
        
        
                       if NfiAlqCof > 0
                          // &linha = 'S07|'+trim(str(NfiTotPrd,15,2))    //   N     15  2       Valor da Base de Calculo da COFINS // Alterado dia 21/12/2017: Base do COFINS está sem o desconto, frete, acréscimo (NfiVlrCof/NfiAlqCof*100)
                          &linha = 'S07|'+trim(str(NfiBseClcCOFINS,15,2))    //   N     15  2       Valor da Base de Calculo da COFINS
                          &linha += '|'+trim(str(NfiAlqCof,5,2))            //   N      5  2       Aliquota da COFINS(Percentual) 
                          do 'tira_car'
                       else
                          &linha = 'S07|0.00'                           //   N     15  2       Valor da Base de Calculo da COFINS
                          &linha += '|0.00'                             //   N      5  2       Aliquota da COFINS(Percentual) 
                          do 'tira_car'
                       endif                             
                endcase

                // UB. Tributos Devolvidos (para o item da NF-e)                                                                                          
                if &NfsTipTrnNOP = 3 or &NfsTipTrnNOP = 4
                    &Linha = 'U50|'+trim(str(NfiPerDev,5,2))+'|'+trim(str(NfiVlrIpiDev,10,2))
                    do 'tira_car'
                endif

       endfor //item da nota

       &TotImp = NfsTotTribFedNac + NfsTotTribFedImp + NfsTotTribEst + NfsTotTribMun

       // TOTALIZADORES
       &linha = 'W|'
       &ret = DFWPTxt(&Linha,1000)
       &ret = DFWNext() 

       &linha = 'W02|'+trim(str(NfsBseClcIcms,15,2))        //   N    15  2    Base de Calculo do ICMS
       &linha += '|'+trim(str(NfsVlrIcms,15,2))             //   N    15  2    Valor total do ICMS
       &linha += '|'+trim(str(NfsBseClcSt,15,2))            //   N    15  2    Base de calculo do ICMS ST
       &linha += '|'+trim(str(NfsVlrSt,15,2))               //   N    15  2    Valor Total do ICMS ST
       &linha += '|'+trim(str(NfsVlrTotPrd,15,2))           //   N    15  2    Valor Total dos produtos e serviços
       &linha += '|'+trim(str(NfsVlrFrt,15,2))              //   N    15  2    Valor Total do Frete
       &linha += '|0.00'                                    //   N    15  2    vSeg) Valor Total do Seguro
       &linha += '|'+trim(str(NfsVlrDsc,15,2))              //   N    15  2    Valor Total do Desconto
       &linha += '|0.00'                                    //   N    15  2    Valor Total do II
       &linha += '|'+trim(str(NfsVlrIpi,15,2))              //   N    15  2    Valor Total do IPI
       &linha += '|'+trim(str(NfsVlrPis,15,2))              //   N    15  2    Valor do PIS
       &linha += '|'+trim(str(NfsVlrCofins,15,2))           //   N    15  2    Valor do COFINS
       &linha += '|'+trim(str(NfsOutDsp,12,2))              //   N    15  2    Outras Despesas acessorias
       &linha += '|'+trim(str(NfsVlrTotNf,15,2))            //   N    15  2    Valor Total da NF-e      
       &linha += '|'+trim(str(&TotImp,15,2))                //   N    15  2  Valor Total dos Tributos
       &linha += '|'+trim(str(NfsDesIcms,15,2))             //   N    15  2  Valor Total do ICMS Desonerado
       &linha += '|'+trim(str(NfsVlrFCP,15,2))              //   N    15  2 (vFCPUFDest)      Valor total do ICMS relativo Fundo de Combate a Pobreza (FCP) da UF de destino
       &linha += '|'+trim(str(NfsVlrIcmsDest,15,2))         //   N    15  2 (vICMSUFDest)     Valor total do ICMS Interestadual para a UF de destino
       &linha += '|'+trim(str(NfsVlrIcmsOri,15,2))          //   N    15  2 (vICMSUFRemet)    Valor total do ICMS Interestadual para a UF do remetente
       &linha += '|'                                        //   N    13  2 (vFCP)            Valor Total do Fundo de Combate a Pobreza
       &linha += '|'+trim(str(NfsVlrFCPSub,15,2))           //   N    13  2 (vFCPST)          Valor do Fundo de Combate a Pobreza Retido por ST
       &linha += '|0.00'                                    //   N    13  2 (vFCPSTRet)       Valor do Fundo de Combate a Pobreza Retido Anteriormente
       &linha += '|'+trim(str(NfsVlrIPIDev,15,2))           //   N    13  2 (vIPIDevol)       Valor Total do IPI Devolvido 
       
       &ret = DFWPTxt(&Linha,1000)
       &ret = DFWNext() 
       
       // Transporte
       &linha = 'X|'+trim(str(NfsTpFrt,1))+'|'                  //   N    1        Modalidade do frete   
       do 'tira_car'
    
       if not null(NfsTrpCod)

            &NfsTrpCod = NfsTrpCod
            do'Transportadora'        
           
           &linha = 'X03|'+trim(&TrpNom)                         //   C    60       Razão Social      
           &linha += '|'+trim(&ForIes)                           //   C    14       Inscrição Estadual
           &linha += '|'+trim(&TrpEndCompleto)                   //   C    60       Endereço Completo
           &linha += '|'+trim(&ForUfCod)                         //   C     2       UF
           &linha += '|'+trim(&ForCidNom)                        //   C    60       Nome do Municipio
           &linha += '|'+trim(Substr(&ForEmail,1,80))            //   C    80       Email do Transportador
           do 'tira_car'
           
           if &ForTp = 'J' and not Null(&ForCnpj)
               &linha = 'X04|'+trim(&ForCnpj)                    //   C    14       CNPJ
               do 'tira_car'
           else
               If not null(&ForCpf)
                   &linha = 'X05|'+trim(&ForCpf)                 //   C    11       CPF
                   do 'tira_car'
               EndIf
           endif           

           if not null(NfsPlcVei)
 
              &NfsPlcVei = NfsPlcVei
              &NfsPlcVei = Strreplace(&NfsPlcVei,'-','')
              &NfsPlcVei = Strreplace(&NfsPlcVei,'.','')
              &NfsPlcVei = Strreplace(&NfsPlcVei,'/','')
              &NfsPlcVei = Strreplace(&NfsPlcVei,'(','')
              &NfsPlcVei = Strreplace(&NfsPlcVei,')','')
              &NfsPlcVei = Strreplace(&NfsPlcVei,'{','')
              &NfsPlcVei = Strreplace(&NfsPlcVei,'}','')

              if not null(&NfsPlcVei) and &IdDest = '1'   // Se a placa não estiver nula e é operação interna
                  &Linha  = 'X18|'+&NfsPlcVei.Trim()
                  &Linha += '|'+NfsVeiUf 
                  &Linha += '|'+NfsAntt+'|'
                  do 'tira_car'
              endif

           endif
       endif
      

       // Volumes
       if not null(NfsQtd)
           &linha = 'X26|'+trim(str(NfsQtd,15))             //   N  15    Quantidade de volumes transportados
           &linha += '|'+trim(NfsEsp)                       //   C  60    Especie dos volumes transportados
           &linha += '|'+trim(NfsMarca)                     //   C  60    Marca dos volumes transportados
           &linha += '|'+trim(NfsNumeracao)                 //   C  60    Numeração dos volumes transportados
           &linha += '|'+trim(str(NfsPesoLiquido,15,3))     //   N  15  3 Peso Liquido(KG)
           &linha += '|'+trim(str(NfsPesoBruto,15,3))       //   N  15  3 Peso Bruto(KG)
           do 'tira_car'
      endif


      // Cobrança
      &NfpSeqFtr = 0

      For Each (NfpSeq)
         &NfpSeqFtr = NfpSeq 
         exit
      endfor       
         
      &linha = 'Y|'
      &ret = DFWPTxt(&Linha,1000)
      &ret = DFWNext()     

      if &NfpSeqFtr > 0 // Se tiver parcelas gera Grupo Duplicatas (Y07) e Grupo Informações de Pagamento (YA)

          &linha = 'Y02|' +(Trim(Str(NfsNum)))           //   C  60         Numero da Fatura
          &linha += '|'+ trim(str(NfsTotPar,15,2))     //   N  15  2      Valor Original da Fatura
          &linha += '|'                                  //   N  15  2      Valor do desconto
          &linha += '|'   +trim(str(NfsTotPar,15,2))   //   N  15  2      Valor Liquido da Fatura
          do 'tira_car'

          &SdtNfPar.Clear()

          for each NfpSeq // Grupo Duplicatas (Y07)

              &Dia             =  NullValue(&Dia)
              &Mes             =  NullValue(&Mes)
              &Ano             =  NullValue(&Ano)
              &Data_Formatada  =  NullValue(&Data_Formatada)
        
              &Dia             =  Padl(Trim(Str(Day(NfpVct),2,0)),2,'0')
              &Mes             =  Padl(Trim(Str(Month(NfpVct),2,0)),2,'0')
              &Ano             =  Str(Year(NfpVct),4,0)
        
              &Data_Formatada  =  &Ano + "-" + &Mes + '-' + &Dia    

              &Venc = &Data_Formatada
                
              &linha  = 'Y07|' + Padl(Trim(Str(NfpSeq)),3,'0')  //   C  60      Numero da Duplicata              
              &linha += '|'   +trim(&Venc)                      //   D          Data do vencimento
              &linha += '|'   +trim(str(NfpVlr,15,2))           //   N  15  2   Valor da duplicata

              &TiraBarra = 'N'
              do 'tira_car'
              &TiraBarra = 'S'  
         endfor

         For Each NfpSeq // Grupo Informações de Pagamento (YA)
               &Flag = 'N' 

               &NfpFormCod = NfpFormCod
               do'FormaPagamento' // Busca a forma de pagamento NFque está relacionada na forma de pagamento
    
                for &SdtNfParItem in &SdtNfPar
                    if &SdtNfParItem.FormPgtoNF = &FormPgtoNF 
                                     
                       &SdtNfParItem.NfpVlr += NfpVlr
                       &Flag = 'S'
                    endif
                endfor
                                
                if &Flag = 'N'
                   &SdtNfParItem = new SdtNfPar.SdtNfParItem()
                   &SdtNfParItem.FormPgtoNF = &FormPgtoNF
                   &SdtNfParItem.NfpVlr     = NfpVlr
                   &SdtNfPar.Add(&SdtNfParItem)
                endif
         endfor
                             
         for &SdtNfParItem in &SdtNfPar                                                                                                
             &linha  = 'YA|' + &SdtNfParItem.FormPgtoNF                                                     // N    2     (tPag)           Forma de Pagamento     
             &linha += '|'   + trim(str(&SdtNfParItem.NfpVlr,15,2))                                         // N   13 2   (vPag)           Valor do Pagamento
             &linha += '|0.00'                                                                              // N   13 2   (vTroco)         Valor do Troco
             &linha += '|'                                                                                  // N    1     (tpIntegra)      Tipo de Integração para o Equipamento
             &linha += '|'                                                                                  // C   14     (CNPJ)           CNPJ da Credenciadora de cartão de crédito e/ou débito
             &linha += '|'                                                                                  // N    2     (tBand)          Bandeira da operadora de cartão de crédito e/ou débito
             &linha += '|'                                                                                  // C   20     (cAut)           Número de autorização da operação cartão de crédito e/ou débito 
             &linha += '|'   + trim(&Pagamento)                                                             // N    1     (IndPag)         Indicador da Forma de Pagamento
             do 'tira_car'                                                                                                                                                                       
         endfor 
      
      Else // Se não tiver parcelas coloca a forma de pagamento 90: Sem Pagamento
            &linha  = 'YA|90'                                                                              // N    2     (tPag)           Forma de Pagamento        90 = Sem Pagamento
            &linha += '|0.00'                                                                              // N   13 2   (vPag)           Valor do Pagamento
            &linha += '|0.00'                                                                              // N   13 2   (vTroco)         Valor do Troco
            &linha += '|'                                                                                  // N    1     (tpIntegra)      Tipo de Integração para o Equipamento
            &linha += '|'                                                                                  // C   14     (CNPJ)           CNPJ da Credenciadora de cartão de crédito e/ou débito
            &linha += '|'                                                                                  // N    2     (tBand)          Bandeira da operadora de cartão de crédito e/ou débito
            &linha += '|'                                                                                  // C   20     (cAut)           Número de autorização da operação cartão de crédito e/ou débito 
            do 'tira_car'                                                                                                                                              
      EndIf

         //Informações adicionais
         &InfAdc = ''

// Alteração 19/12/2017: As observações fiscais passaram a ser gravadas na PGeraNota
//         if NfsVlrIcmsDest > 0
//            &InfAdc += 'VALOR DO ICMS DEVIDO A UF DE DESTINO: '+Trim(str(NfsVlrIcmsDest,10,2)) 
//         endif
//
//         if not null(&CliCnpjEndEnt) or Not null(&CliCpfEndEnt) 
//            if not null(&CliEndEnt)
//    
//                &InfAdc += 'Endereco de Entrega: ' + &CliEndEnt.Trim()+','+&CliEndNumEnt.Trim()+','+&CliEndComplEnt.Trim()+'-'+&CliEndBaiEnt.Trim()+','+&CliCidNomEnt.Trim()+'-'+&CliUfCodEnt.Trim()+'/'
//
//            endif
//         endif
//
//         
//         &InfAdc += ' Pedido = ' + trim(str(NfsNumPed)) + ' /'
//
//         If Not Null(NfsVlrDsc)
//            &InfAdc += ' SUFRAMA: ' + Trim(ToformattedString(&CliInsSuf)) + ' / '
//         Endif
  
         If Not Null(NfsInfCmp)
             &InfAdc += trim(NfsInfCmp) + ' / '
         Endif

// Alteração 19/12/2017: As observações fiscais passaram a ser gravadas na PGeraNota
//         if NfsDesIcms > 0
//            &InfAdc += ' DESCONTO AREA DE LIVRE COMERCIO:'+TRIM(STR(NfsDesIcms,12,2))+' / '
//         endif
//
//         if NfsDesCofins > 0
//            &InfAdc += ' DESCONTO COFINS:'+TRIM(STR(NfsDesCofins,12,2))+' / '
//         endif
//
//         if NfsDesPis > 0
//            &InfAdc += ' DESCONTO PIS:'+TRIM(STR(NfsDesPis,12,2))+' / '
//         endif  

         do 'TiraCar'
         &InfAdc = ''
         &InfAdc = trim(&linha1)
         &linha = 'Z|' +''                 //   C  256        Informações Adicionais de Interesse do Fisco
         &linha += '|' +trim(&InfAdc)      //   C  5000       Informações Complementares de interesse do Contriuinte
         &ret = DFWPTxt(&Linha,1000)
         &ret = DFWNext()

  Endfor   // Fim do For nota fiscal

endfor // Fim do For da SDT de Notas

EndSub // Fim da Sub 'MontaArquivo'


Sub 'Tiracar'

    &Linha1 = ""

    for  &i = 1 to len(trim(&InfAdc))
    
        &caracter=substr(&InfAdc,&i,1)  
    
       do case
    //           case asc(&caracter) > 96 .and.
    //                asc(&caracter) < 126
    //                &caracter=upper(&caracter)
               case asc(&caracter) = 225 .or.
                    asc(&caracter) = 193 .or.
                    asc(&caracter) = 224 .or.
                    asc(&caracter) = 192 .or.
                    asc(&caracter) = 227 .or.
                    asc(&caracter) = 195 .or.
                    asc(&caracter) = 194 .or.
                    asc(&caracter) = 196
                    &caracter="A"
               case asc(&caracter) = 233 .or.
                    asc(&caracter) = 201 .or.
                    asc(&caracter) = 200 .or.
                    asc(&caracter) = 232 .or.
                    asc(&caracter) = 203 
                    &caracter="E"
               case asc(&caracter) = 237 .or.
                    asc(&caracter) = 205 .or.
                    asc(&caracter) = 204 .or.
                    asc(&caracter) = 236
                    &caracter="I"
               case asc(&caracter) = 243 .or.
                    asc(&caracter) = 211 .or.
                    asc(&caracter) = 242 .or.
                    asc(&caracter) = 210 .or.
                    asc(&caracter) = 245 .or.
                    asc(&caracter) = 213 .or.
                    asc(&caracter) = 226 .or.
                    asc(&caracter) = 214 .or.
                    asc(&caracter) = 212
                    &caracter="O"
               case asc(&caracter) = 250 .or.
                    asc(&caracter) = 217 .or.
                    asc(&caracter) = 218 .or.
                    asc(&caracter) = 249
                    &caracter="U"
               case asc(&caracter) = 231 .or.
                    asc(&caracter) = 199
                    &caracter="C"
               case asc(&caracter) = 241 .or.
                    asc(&caracter) = 209
                    &caracter="N"
               case asc(&caracter) = 186 .or.
                    asc(&caracter) = 187
                    &caracter="." 
               case asc(&caracter) = 10 .or. asc(&caracter) = 13
                    &Caracter = ' '
            endcase 
        
       
            if &caracter = "´"                    
               &caracter="."
            endif
        
            if &caracter = "º"                    
               &caracter="o"
            endif
        
            if &caracter = "Ê"                    
               &caracter="E"
            endif
    
         if &caracter = "&"
               &caracter = "E"
            endif
    
            if &caracter = "?"
              &caracter = ""
            endif
    
            if &caracter  = "'"
               &caracter = ""
            endif
    
            if &caracter = '"'
               &caracter = ""
            endif
    
            if &caracter = '#'
               &caracter = ""
            endif
    
            if &caracter = "*"
               &caracter = ""
            endif
    
            if &caracter = "!"
               &caracter = ""
            endif
               
            if &caracter = "ª"                    
               &caracter="a"
            endif
        
            if &caracter = "Ê"                    
               &caracter="E"
            endif
    
            if &caracter = "Ô"
               &caracter = "O"
            endif
    
            if &caracter = "°"
               &caracter = ""
            endif
    
    //        if &caracter = "$"
    //           &caracter = ""
    //        endif
    
    //        if &caracter = '@'
    //           &caracter = ""
    //        endif
    
            if &caracter = '+'
               &caracter = ""
            endif
    
    //        if &caracter = ","
    //           &caracter = ""
    //        endif
    
    //        if &caracter = '_'
    //           &caracter = ""
    //        endif
    
            if &caracter = '´'
               &caracter = ""
            endif
    
            if &caracter = "Â"
               &caracter = "A"
            endif
    
        &linha1 += &caracter
       
    endfor 
endsub


Sub 'Tira_car'
   &Linha1 = ""

    for  &i = 1 to len(trim(&Linha))
    
        &caracter=substr(&Linha,&i,1)  
    
       do case
    //           case asc(&caracter) > 96 .and.
    //                asc(&caracter) < 126
    //                &caracter=upper(&caracter)
               case asc(&caracter) = 225 .or.
                    asc(&caracter) = 193 .or.
                    asc(&caracter) = 224 .or.
                    asc(&caracter) = 192 .or.
                    asc(&caracter) = 227 .or.
                    asc(&caracter) = 195 .or.
                    asc(&caracter) = 194 .or.
                    asc(&caracter) = 196
                    &caracter="A"
               case asc(&caracter) = 233 .or.
                    asc(&caracter) = 201 .or.
                    asc(&caracter) = 200 .or.
                    asc(&caracter) = 232 .or.
                    asc(&caracter) = 203 
                    &caracter="E"
               case asc(&caracter) = 237 .or.
                    asc(&caracter) = 205 .or.
                    asc(&caracter) = 204 .or.
                    asc(&caracter) = 236
                    &caracter="I"
               case asc(&caracter) = 243 .or.
                    asc(&caracter) = 211 .or.
                    asc(&caracter) = 242 .or.
                    asc(&caracter) = 210 .or.
                    asc(&caracter) = 245 .or.
                    asc(&caracter) = 213 .or.
                    asc(&caracter) = 226 .or.
                    asc(&caracter) = 214 .or.
                    asc(&caracter) = 212
                    &caracter="O"
               case asc(&caracter) = 250 .or.
                    asc(&caracter) = 217 .or.
                    asc(&caracter) = 218 .or.
                    asc(&caracter) = 249
                    &caracter="U"
               case asc(&caracter) = 231 .or.
                    asc(&caracter) = 199
                    &caracter="C"
               case asc(&caracter) = 241 .or.
                    asc(&caracter) = 209
                    &caracter="N"
               case asc(&caracter) = 186 .or.
                    asc(&caracter) = 187
                    &caracter="."  
               case asc(&caracter) = 10 .or. asc(&caracter) = 13
                    &Caracter = ' '
            endcase 
        
       
            if &caracter = "´"                    
               &caracter="."
            endif
            if &caracter = "ª"                    
               &caracter="a"
            endif
        
            if &caracter = "º"                    
               &caracter=" "
            endif
        
            if &caracter = "Ê"                    
               &caracter="E"
            endif
    
         if &caracter = "&"
               &caracter = "E"
            endif
    
            if &caracter = "?"
              &caracter = ""
            endif
    
            if &caracter  = "'"
               &caracter = ""
            endif
    
            if &caracter = '"'
               &caracter = ""
            endif
    
    //        if &caracter = ":"
    //           &caracter = ""
    //        endif
    
            if &caracter = '#'
               &caracter = ""
            endif
    
            if &caracter = "*"
               &caracter = ""
            endif
    
            If &TiraBarra = 'S' // Permite barra na Tag Y07
                if &caracter = '/'
                   &caracter = ""
                endif
            EndIf
    
            if &caracter = "!"
               &caracter = ""
            endif
               
            if &caracter = "ª"                    
               &caracter="a"
            endif
        
            if &caracter = "Ê"                    
               &caracter="E"
            endif
    
            if &caracter = "Ô"
               &caracter = "O"
            endif
    
            if &caracter = "°"
               &caracter = ""
            endif
    //
    //        if &caracter = "$"
    //           &caracter = ""
    //        endif
    
    //        if &caracter = '('
    //           &caracter = ""
    //        endif
    //
    //        if &caracter = ')'
    //           &caracter = ""
    //        endif
    
    //        if &caracter = '@'
    //           &caracter = ""
    //        endif
    
            if &caracter = '+'
               &caracter = ""
            endif
    //
    //        if &caracter = '%'
    //           &caracter = ""
    //        endif
    
    //        if &caracter = ","
    //           &caracter = ""
    //        endif
    
    //        if &caracter = '_'
    //           &caracter = ""
    //        endif
    
            if &caracter = '´'
               &caracter = ""
            endif
    //
    //        if &caracter = ';'
    //           &caracter= ""
    //        endif
    
            if &caracter = "Â"
               &caracter = "A"
            endif
    
        &linha1 += &caracter        
    endfor    
    
    &Linha = Trim(&linha1) 
    &ret = DFWPTxt(&Linha,1000)
    &ret = DFWNext()
EndSub


Sub'FormaPagamento'
    For Each FormCod
        Where FormCod = &NfpFormCod
            &FormPgtoNF = FormPgtoNF
    EndFor

    If Null(&FormPgtoNF) // Se não estiver relacionado nenhuma Forma de Pagamento NF no cadastro de Forma de Pagamento, considerar como 15: Boleto Bancário.
        &FormPgtoNF = '15' // Boleto Bancário
    EndIf
EndSub


sub 'DadosPedido'
    &PedCondCod = 0
    
    For each
        where PedCod = &NfsNumPed
    
            &PedCondCod = PedCondCod
            &PedOrdCompra = PedOrdCompra
            &PedIndPres = PedIndPres
            &PedIndIntermed = PedIndIntermed            
            &PedCnpjIntermed = PedCnpjIntermed
            &PedIdCadIntTran = PedIdCadIntTran
    endfor 
    
    For Each
        Where CondCod = &PedCondCod
            
            If CondTp = 'V'
                &Pagamento ='0'//a vista
            Else
                &Pagamento = '1'//a prazo
            EndIf           
    endfor
endSub


Sub 'Cliente'
    For each
        Where CliCod = &NfsCliCod
             
        &CliNom = CliNom
        &CliCnpj = CliCnpj
        &CliIes = CliIes
        &CliPassaporte = CliPassaporte
        &CliCpf = CliCpf
        &CliRg = CliRg
        &CliEnd = CliEnd
        &CliEndNum = CliEndNum
        &CliEndBai = CliEndBai
        &CliCidCod = CliCidCod
        &CliCidNom = CliCidNom
        &CliUfCod  = CliUfCod
        &CliEndComp = CliEndComp
        &CliEndEnt = CliEndEnt
        &CliEndNumEnt = CliEndNumEnt
        &CliEndBaiEnt = CliEndBaiEnt
        &CliCidCodEnt = CliCidCodEnt
        &CliCidNomEnt = CliCidNomEnt
        &CliUfCodEnt  = CliUfCodEnt
        &CliEndComplEnt = CliEndComplEnt
        &CliCepEnt = CliCepEnt
        &CliCep = CliCep
        &CliFone = CliFone
        &CliEmail = CliEmail
        &CliTp = CliTp
        &CliInsSuf = CliInsSuf
        &CliIM = CliIM
        &CliTp2 = CliTp2
        &CliCnpjEndEnt = CliCnpjEndEnt
        &CliCpfEndEnt  = CliCpfEndEnt
        &CliIeIse = CliIEIse
        &CliCel = CliCel
        &CliInsProdRural = CliInsProdRural
        &CliIeEndEnt = CliIeEndEnt
        &CliRzEndEnt = CliRzEndEnt
        &CliEmailEndEnt = CliEmailEndEnt
        &CliFoneEndEnt = CliFoneEndEnt
    
        if CliIEIse = 'S'
           &CliIes = ''
        endif
    
        &CliTpEndSeq = CliTpEndSeq
        &CliTpEndEntSeq = CliTpEndEntSeq
        do'tipoend'
    
        &CliCnpj = strreplace(&CliCnpj,'-','')
        &CliCnpj = strreplace(&CliCnpj,'.','')
        &CliCnpj = strreplace(&CliCnpj,'/','')
        &CliCnpj = strreplace(&CliCnpj,'\','')
        &CliCnpj = strreplace(&CliCnpj,',','')
        &CliCnpj = strreplace(&CliCnpj,' ','')
    
        &CliCnpjEndEnt = strreplace(&CliCnpjEndEnt,'-','')
        &CliCnpjEndEnt = strreplace(&CliCnpjEndEnt,'.','')
        &CliCnpjEndEnt = strreplace(&CliCnpjEndEnt,'/','')
        &CliCnpjEndEnt = strreplace(&CliCnpjEndEnt,'\','')
        &CliCnpjEndEnt = strreplace(&CliCnpjEndEnt,',','')
        &CliCnpjEndEnt = strreplace(&CliCnpjEndEnt,' ','')
    
        &CliCpf = strreplace(&CliCpf,'-','')
        &CliCpf = strreplace(&CliCpf,'.','')
        &CliCpf = strreplace(&CliCpf,'/','')
        &CliCpf = strreplace(&CliCpf,'\','')
        &CliCpf = strreplace(&CliCpf,',','')
        &CliCpf = strreplace(&CliCpf,' ','')
    
        &CliCpfEndEnt = strreplace(&CliCpfEndEnt,'-','')
        &CliCpfEndEnt = strreplace(&CliCpfEndEnt,'.','')
        &CliCpfEndEnt = strreplace(&CliCpfEndEnt,'/','')
        &CliCpfEndEnt = strreplace(&CliCpfEndEnt,'\','')
        &CliCpfEndEnt = strreplace(&CliCpfEndEnt,',','')
        &CliCpfEndEnt = strreplace(&CliCpfEndEnt,' ','')
    
        &CliIes = strreplace(&CliIes,'-','')
        &CliIes = strreplace(&CliIes,'.','')
        &CliIes = strreplace(&CliIes,'/','')
        &CliIes = strreplace(&CliIes,'\','')
        &CliIes = strreplace(&CliIes,',','')
        &CliIes = strreplace(&CliIes,' ','')

        &CliIeEndEnt = strreplace(&CliIeEndEnt,'-','')
        &CliIeEndEnt = strreplace(&CliIeEndEnt,'.','')
        &CliIeEndEnt = strreplace(&CliIeEndEnt,'/','')
        &CliIeEndEnt = strreplace(&CliIeEndEnt,'\','')
        &CliIeEndEnt = strreplace(&CliIeEndEnt,',','')
        &CliIeEndEnt = strreplace(&CliIeEndEnt,' ','')

        &CliInsProdRural = strreplace(&CliInsProdRural,'-','')
        &CliInsProdRural = strreplace(&CliInsProdRural,'.','')
        &CliInsProdRural = strreplace(&CliInsProdRural,'/','')
        &CliInsProdRural = strreplace(&CliInsProdRural,'\','')
        &CliInsProdRural = strreplace(&CliInsProdRural,',','')
        &CliInsProdRural = strreplace(&CliInsProdRural,' ','')
    
        &CliCep = strreplace(&CliCep,'-','')
        &CliCep = strreplace(&CliCep,' ','')

        &CliCepEnt = strreplace(&CliCepEnt,'-','')
        &CliCepEnt = strreplace(&CliCepEnt,' ','')
    
        &CliFone = strreplace(&CliFone,'-','')
        &CliFone = strreplace(&CliFone,')','')
        &CliFone = strreplace(&CliFone,'(','')
        &CliFone = strreplace(&CliFone,' ','')

        &CliFoneEndEnt = strreplace(&CliFoneEndEnt,'-','')
        &CliFoneEndEnt = strreplace(&CliFoneEndEnt,')','')
        &CliFoneEndEnt = strreplace(&CliFoneEndEnt,'(','')
        &CliFoneEndEnt = strreplace(&CliFoneEndEnt,' ','')
    
        &CliCel = strreplace(&CliCel,'-','')
        &CliCel = strreplace(&CliCel,')','')
        &CliCel = strreplace(&CliCel,'(','')
        &CliCel = strreplace(&CliCel,' ','')
        &CliCel = Trim(substr(&CliCel,1,14))
                 
    EndFor

    for each
       where UfCod = &CliUfCod
    
           &CliPaiCod = UfPaiCod
           &CliPaiNom = UfPaiNom    
    endfor
    
    for each
       where UfCod = &CliUfCodEnt
    
           &CliPaiCodEnt = UfPaiCod
           &CliPaiNomEnt = UfPaiNom    
    endfor
EndSub


sub'Transportadora'
    for each
        where TrpCod = &NfsTrpCod
    
            &TrpNom = Substr(TrpNom,1,60)
            &ForCnpj = TrpCnpj
            &ForIes = TrpIe
            &ForCpf = TrpCpf
            &ForRG = TrpRG
            &TrpEndCompleto = Trim(TrpEnd)

            If not null(TrpEndNum)
                &TrpEndCompleto +=  ', ' + Trim(TrpEndNum)
            EndIF

            &ForEndNum = TrpEndNum
            &ForEndBai = TrpEndBai
            &ForCep = TrpEndCep
            &ForCidCod = TrpCidCod
            &ForCidNom = TrpCidNom
            &ForUfCod = TrpUFCod
            &ForEndCompl = TrpEndCmp
            &ForFone = TrpFone
            &ForEmail = TrpEmail
            &ForTp = TrpTp
        
            &ForCnpj = strreplace(&ForCnpj,'-','')
            &ForCnpj = strreplace(&ForCnpj,'.','')
            &ForCnpj = strreplace(&ForCnpj,'/','')
            &ForCnpj = strreplace(&ForCnpj,'\','')
            &ForCnpj = strreplace(&ForCnpj,',','')
            &ForCnpj = strreplace(&ForCnpj,' ','')
        
            &ForCpf = strreplace(&ForCpf,'-','')
            &ForCpf = strreplace(&ForCpf,'.','')
            &ForCpf = strreplace(&ForCpf,'/','')
            &ForCpf = strreplace(&ForCpf,'\','')
            &ForCpf = strreplace(&ForCpf,',','')
            &ForCpf = strreplace(&ForCpf,' ','')
        
            &ForIes = strreplace(&ForIes,'-','')
            &ForIes = strreplace(&ForIes,'.','')
            &ForIes = strreplace(&ForIes,'/','')
            &ForIes = strreplace(&ForIes,'\','')
            &ForIes = strreplace(&ForIes,',','')
            &ForIes = strreplace(&ForIes,' ','')
        
            &ForCep = strreplace(&ForCep,'-','')                
    endfor
endsub


Sub 'opr'
    for each
       where CfopSeq = &NfsCfopSeq
            &CfopFinEmi = CfopFinEmi
    endfor
endsub


sub'cfop'
    for each
       where CfopSeq = &NfiCfopSeq
         &CfopModBseIcms = CfopModBseIcms
         &CfopModBseIcmsSt = CfopModBseIcmsSt
         &CfopBseIcms = CfopBseIcms
         &CfopBseIcmsSt = CfopBseIcmsSt
         &CfopEnqCod = CfopEnqCod 
         &CfopMotDesoneracao = CfopMotDesoneracao         
    
    endfor
    
    for each
       where PrdCod = &PrdCod
    
          &PrdcEan = PrdcEan.Trim()
          &PrdcEanTrib = PrdcEanTrib.Trim()
          &PrdCodBarras = PrdCodBarras.Trim()
    
           do case
               case &CliUfCod = 'AC'
                  &PerSt = PrdAcMva
               case &CliUfCod = 'AL'
                  &PerSt = PrdAlMva
               case &CliUfCod = 'AP'
                  &PerSt = PrdApMva
               case &CliUfCod = 'AM'
                  &PerSt = PrdAmMva
               case &CliUfCod = 'BA'
                  &PerSt = PrdBaMva
               case &CliUfCod = 'CE'
                  &PerSt = PrdCeMva
               case &CliUfCod = 'DF'
                  &PerSt = PrdDfMva
               case &CliUfCod = 'ES'
                  &PerSt = PrdEsMva
               case &CliUfCod = 'GO'
                  &PerSt = PrdGoMva
               case &CliUfCod = 'MG'
                  &PerSt = PrdMgMva
               case &CliUfCod = 'MT'
                  &PerSt = PrdMtMva
               case &CliUfCod = 'MS'
                  &PerSt = PrdMsMva
               case &CliUfCod = 'PA'
                  &PerSt = PrdPaMva
               case &CliUfCod = 'PB'
                  &PerSt = PrdPbMva
               case &CliUfCod = 'PI'
                  &PerSt = PrdPiMva
               case &CliUfCod = 'PR'
                  &PerSt = PrdPrMva 
               case &CliUfCod = 'RJ'
                  &PerSt = PrdRjMva            
               case &CliUfCod = 'RS'
                  &PerSt = PrdRsMva
               case &CliUfCod = 'RN'
                  &PerSt = PrdRnMva
               case &CliUfCod = 'RR'
                  &PerSt = PrdRrMva
               case &CliUfCod = 'RO'
                  &PerSt = PrdRsMva
               case &CliUfCod = 'SC'
                  &PerSt = PrdScMva
               case &CliUfCod = 'SP'
                  &PerSt = PrdSpMva
               case &CliUfCod = 'TO'
                  &PerSt = PrdToMva
               case &CliUfCod = 'SE'
                  &PerSt = PrdSeMva
           endcase   
    endfor
    
    // Alteração dia 26/12/2017: (Convenio ICMS 52/2017) segundo Escritório Contábil Cunha – A partir de 01/01/2018 (Se for verdadeira todas as condições abaixo, zera a % do MVA pois não utiliza no novo cálculo)
    If &CliTp = 'J' and &CliIeIse = 'C' and not Null(&CliIes) and &NfsConsFinal = 'S' and &EmpUf <> &CliUfCod
        &PerSt = 0
    EndIf

endsub


sub'tipo'
    for each
        where TpEndSeq = &EmpTpEndSeq
            &TipoEnd = TpEndDsc.Trim()
    endfor
endsub


sub'tipoend'
    for each
       where TpEndSeq = &CliTpEndSeq
       &CliTipoEnd = TpEndDsc.Trim()
    endfor
    
    for each
       where TpEndSeq = &CliTpEndEntSeq
       &CliTipoEndEnt = TpEndDsc.Trim()
    endfor
endsub


sub'Autorizações'
    for each
       where EmpCod = &Logon.EmpCod    
        
           &EmpAutDoc = EmpAutDoc
           &EmpAutDoc = StrReplace(&EmpAutDoc,'-','')
           &EmpAutDoc = StrReplace(&EmpAutDoc,',','')
           &EmpAutDoc = StrReplace(&EmpAutDoc,'*','')
           &EmpAutDoc = StrReplace(&EmpAutDoc,'.','')
           &EmpAutDoc = StrReplace(&EmpAutDoc,'#','')
           &EmpAutDoc = StrReplace(&EmpAutDoc,'/','')
           &EmpAutDoc = StrReplace(&EmpAutDoc,' ','')
                   
           if EmpAutTp = 'J'
              &Linha = 'G50A|'+&EmpAutDoc.Trim()
           else
              &Linha = 'G50B|'+&EmpAutDoc.Trim()
           endif
        
           do 'tira_car'    
    endfor
endsub


sub'cest'

    for each
        where PrdCod = &PrdCod    
            &NcmCod = PrdNcmCod
            &NcmEx  = PrdNcmEx
    endfor
    
    for each
        where NcmCod = &NcmCod
        where NcmEx = &NcmEx
    
        &CestCod = CestCod
        &CestCod = strreplace(&CestCod,'.','')
    endfor
endsub

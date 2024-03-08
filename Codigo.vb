
for each
   where AgeCod = &CcrAgeCod
   where BanCod = &CcrBanCod
   where CceCod = &CcrCceCod

   CceSeqBol += 1
   &CceSeqBol = CceSeqBol

   &CceCodEmp = CceCodEmp
   &AgeDig = AgeDig
   &AgeCod = AgeCod

   &CceTpRem = CceTpRem

   &Agencia = padl(trim(str(AgeCod)),4,'0')

   &CceCodEmp = CceCodEmp.Trim()
   for &i = 1 to len(&CceCodEmp)
 
       if substr(&CceCodEmp,&i,1) <> '-'
          &CodEmp += substr(&CceCodEmp,&i,1)
       else 
          &pos = &i
          exit
       endif

   endfor

   &CodEmp = padl(trim(&CodEmp),9,'0')

   if &pos > 0
 
      &pos += 1

      &DVCli = substr(&CceCodEmp,&pos,1)

   endif

endfor

for each
    where CcrSeq = &Seq

    &Parcela = padl(trim(str(CcrPar)),2,'0')

endfor

//    if &CceTpRem = 1
       &CceSeqBol2 = PADL(TRIM(STR(&CceSeqBol)),7,'0')
       &ConstCalc  = '319731973197319731973'     // Constante para calculo = 3197
       &LinhaCalcNossoNr = &Agencia + &CodEmp + &DVCli + &CceSeqBol2
       Do 'CalcDVNossoNr'
       &CcrNossNum = &CceSeqBol2.Trim() + &DVNossoNr.Trim()
//    else
//
//        &NossoNumero      =  Padl(Trim(Str(&CceSeqBol,7,0)),7,'0')
//        &DVNossoNr        = ''
//        &ConstCalc        = '319731973197319731973'     // Constante para calculo = 3197
//        &LinhaCalcNossoNr =  &LinhaCalcNossoNr1 + &NossoNumero
//        Do 'CalcDVNossoNr'
//        &CcrNossNum = '00' + &NossoNumero + Trim(&DVNossoNr) +  &Parcela + '01' + '3' + space(05)
//
//    endif
     
    

Sub 'CalcDVNossoNr' // BASE 11
  &intcontador = 0
  &Soma        = 0
         
  //pega cada caracter do numero e multiplica por cada número da constante
  For &intcontador = 1 to Len(&LinhaCalcNossoNr) 
                  
      &ValorLinha     = Val(substr(&LinhaCalcNossoNr, &intcontador, 1))
      &Multiplicador  = Val(substr(&ConstCalc, &intcontador, 1))
      &Soma          += &ValorLinha * &Multiplicador
  Endfor


  &intResto = mod(&Soma,11)

  &subtrair = 11 - &intResto

  if &subtrair > 9
     &DVNossoNr = '0'
  else
     &DigNr = 11 - &intResto
     &DVNossoNr = str(&DigNr)
  endif


//&ValorDiv = Int((&Soma/11))
//&intResto = &Soma - (Int(&ValorDiv) * 11)
//
// Do Case
//    Case &intResto = 0
//        &DVNossoNr = '0'
//    Case &intResto = 1
//        &DVNossoNr = '0'
//    Otherwise
//
//         &DigNr = 11 - &intResto
//         &DVNossoNr = str(&DigNr)
//
// EndCase



EndSub
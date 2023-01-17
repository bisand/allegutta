import React, { Component } from 'react';

export class Vedtekter extends Component<any, any> {
  static displayName = Vedtekter.name;

  constructor(props: any) {
    super(props);
    this.state = {};
  }

  private renderPanel(contentBody: any, title: string) {
    return (
      <div className="card mb-3">
        <div className="card-header">
          {title}
        </div>
        <div className="card-body">
          {contentBody}
        </div>
      </div>
    );
  }

  render() {
    return this.renderPanel(this.renderContent(), "VEDTEKTER FOR AKSJESPAREKLUBBEN ALLEGUTTA");
  }

  private renderContent() {
    return (
      <div>
        {/* <h5>VEDTEKTER FOR AKSJESPAREKLUBBEN ALLEGUTTA</h5> */}
        <h6 id="navn">§ 1 NAVN OG REGISTRERING</h6>
        <p>
          Klubbens navn er ”Aksjespareklubben Allegutta”.
          Det opprettes aksjekonto i en eller flere banker med tilhørende VPS-konto
        </p>
        <h6 id="formaal">§ 2 FORMÅL</h6>
        <p>
          Klubbens formål er: å gi medlemmene innsikt og forståelse for
          aksjemarkedet og næringslivet for øvrig og å bidra til medlemmenes
          personlige sparing ved for deres felles regning å erverve og forvalte
          aksjer. Aksjespareklubben skal også være et sosialt nettverk.
          Med aksjer menes her aksjer, fond og børsnoterte fond som på anskaffelsestidspunktet
          er notert på Oslo Børs. - OBX, OSEBX og Euronext Growth. Det skal ikke handles i
          warrants eller derivater.
        </p>
        <p>
          Siden Aksjespareklubbens medlemmer kan være bundet av Kredittilsynets og
          Finansinspektionens til enhver tid gjeldende regler for egenhandel, gjelder de samme
          regler ved kjøp og salg. Dersom andre medlemmer er forhindret av interne regler som
          gjelder for ansatte i den bedriften medlemmet jobber, som for eksempel at de ikke har
          tillatelse til å investere i spesifikke selskap, så skal Aksjeklubben også overholde disse
          reglene.
        </p>
        <p>
          Aksjespareklubben skal basert på ovenstående ha en kjøp og behold (buy & hold)
          strategi. Aksjespareklubbens øvrige strategi vil i detalj fastsettes av medlemmene i
          fellesskap på medlemsmøter og spesifiseres i eget vedlegg. Vedlegg 1 -
          Investeringsstrategi. Det er ikke krav om 2/3 flertall for å gjøre endringer i vedlegg 1 -
          Investeringsstrategi.
        </p>
        <h6 id="medlemskap">§ 3 MEDLEMSKAP</h6>
        <p>
          Kun personer over 18 år kan opptas som medlemmer. Nye medlemmer kan
          opptas såfremt 2/3 av de eksisterende medlemmer samtykker. Nye medlemmer skal
          legitimeres med kopi av gyldig ID, attestert av godkjent myndighet.
          Hvert medlem eier minst én (1) og ikke flere enn seksten (16) andeler (fra 01.01.22)
          Kun medlemmer kan eie andeler.
        </p>
        <p>
          Oversikt over medlemmene med angivelse av andelsfordeling og tillitsverv
          følger vedtektene som vedlegg A.
        </p>
        <h6 id="kapital">§ 4 KAPITAL</h6>
        <p>
          Klubbens kapital er ved stiftelsen den 29. januar 2014 fordelt på 38 andeler à
          kr. 5.000. Hvert medlem skal innen 29. januar ha innbetalt sin(e) andel(er).
          Til kapitalen legges et kvartalsvis sparebeløp pr. andel som fastsettes
          årlig av årsmøtet med minst ¾-flertall. Sparebeløpet skal innbetales
          første gang innen 1. april 2014 og deretter innen den 1. i hvert
          kvartal. Pengene skal innbetales til den av leder angitte konto, p.t. bankkonto
          9710.32.19128 som disponeres av Bjørnar Næss, som betaler inn til Aksjespareklubbens
          depotkonto så snart alle andelseiere har betalt inn til bankkonto.
          Kapitalen kan økes ved utstedelse av andeler til nye eller eksisterende
          medlemmer og reduseres ved uttreden eller delvis uttreden. Jfr. <a href="#uttreden">§ 11</a> og
          <a href="#delvisuttreden">§ 12</a>.
        </p>
        <h6 id="inntekter">§ 5 INNTEKTER - UTGIFTER</h6>
        <p>
          Alle utbytter, gevinster og andre inntekter skal tillegges kapitalen.
          Klubbens driftsutgifter og eventuelle tap dekkes av kapitalmidlene.
        </p>
        <h6 id="organisasjon">§ 6 ORGANISASJON</h6>
        <ol>
          <li>Årsmøte
            <p>Årsmøte skal avholdes årlig og innen utgangen av oktober. Styret
              innkaller til årsmøtet. Innkalling med saksdokumenter skal sendes
              medlemmene senest to uker før årsmøtet.
            </p>
          </li>
          <p>Følgende saker skal behandles på årsmøtet:
            <ol>
              <li>Styrets beretning</li>
              <li>Fastsettelse av resultatregnskap og balanse</li>
              <li>Revisors rapport</li>
              <li>Fastsettelse av sparebeløp i h.h.t. <a href="#kapital">§ 4</a>, 2. ledd</li>
              <li>Eventuelle vedtektsendringer</li>
              <li>Valg av leder, styre og revisor</li>
            </ol>
          </p>
          <li>Styret</li>
          <p>
            Styret består av leder, samt 2 - 3 medlemmer.
          </p>
          <p>
            Styret velges for to år av gangen.
          </p>
          <p>
            Styret forestår den daglige ledelse og er ansvarlig for føring av
            klubbens regnskaper og protokoller. Styret plikter å ajourføre
            vedtektenes vedlegg A (jfr. <a href="#medlemskap">§ 3</a>), samt underrette bank- og
            fondsmeglerforbindelser om endringer i vedtektene, inkludert vedlegg A.
          </p>
          <li>Medlemsmøter</li>
          <p>
            Styret kan innkalle til medlemsmøte med tre dagers varsel.
            Møtene ledes av lederen, eller, i hans fravær, stedfortrederen.
          </p>
          <p>
            Avgjørelser om kjøp og salg av aksjer fattes på medlemsmøtene eller av
            andre retningslinjer gitt på sist avholdte medlemsmøte.
          </p>
          <p>
            Det kan ikke disponeres over mer enn det til enhver tid er dekning for i
            klubbens aktiva, jfr. <a href="#signaturansvar">§ 7</a>.
          </p>
          <p>Medlemsmøtet er beslutningsdyktig når ½-parten av medlemmene er
            representert. Medlemmer kan gi andre medlemmer fullmakt til å representere disse på
            medlemsmøtet, fullmakt gis ved å sende en mail til styrets leder, jfr. <a href="#votering">§ 9</a>.
          </p>
          <p>
            Møtet behandler spørsmål om opptak av nye medlemmer. Innkalling, med
            opplysninger om søkeren og hvor mange andeler vedkommende ønsker å
            tegne, skal i slike tilfeller sendes medlemmene senest to uker før
            møtet.
          </p>
        </ol>
        <h6 id="signaturansvar">§ 7 SIGNATURANSVAR</h6>
        <p>
          Lederen forplikter klubben.
        </p>
        <p>
          Lederen er bundet av de instrukser hun/han mottar av medlemsmøtet og står
          personlig ansvarlig for tap hun/han måtte påføre klubben ved overtredelse av disse.
        </p>
        <p>
          Medlemmene hefter solidarisk for klubbens forpliktelser (jfr. Vedlegg A).
        </p>
        <h6 id="vedtektsendringer">§ 8 VEDTEKTSENDRINGER</h6>
        <p>
          Vedtektsendringer vedtas med årsmøtet med 2/3-dels flertall.
        </p>
        <h6 id="votering">§ 9 VOTERING</h6>
        <p>
          Hvert medlem har én stemme, uavhengig av antall andeler. Ved stemmelikhet har leder dobbeltstemme.
        </p>
        <p>
          Et medlem kan la seg representere av fullmektig som selv er medlem av klubben.
        </p>
        <p>
          Alle stemmesaker avgjøres ved simpelt flertall blant de representerte
          medlemmene der ikke annet er bestemt.
        </p>
        <h6 id="protokoll">§ 10 PROTOKOLL</h6>
        <p>
          Det skal føres protokoll (referat) fra årsmøte, styremøter og medlemsmøter.
          Protokollen skal til enhver tid være tilgjengelig for klubbens medlemmer.
        </p>
        <h6 id="uttreden">§ 11 UTTREDEN OG EKSKLUSJON - INNTREDEN</h6>
        <ol>
          <li>Uttreden</li>
          <p>
            Uttreden skjer normalt pr. 31. desember og skal skriftlig meddeles
            styret innen 30. september.
          </p>
          <p>
            Den uttredendes andel(er) av aksjebeholdningen beregnes på grunnlag av
            markedsverdien pr. uttredelsesdato. Ved hel eller delvis uttreden etter
            mindre enn 12 måneder medlemskap trekkes 5% fra den uttredendes andel.
            Ved uttreden etter 12 - 24 måneders medlemskap trekkes 2,5% fra.
          </p>
          <p>
            Et medlem kan tre ut i løpet av året dersom vedkommendes andeler,
            rettigheter og plikter etter disse vedtekter, i sin helhet overdras til
            annet (andre) medlem(mer) (jfr. dog <a href="#medlemskap">§ 3</a>, 3. ledd) eller til en
            utenforstående som klubben godkjenner slik <a href="#medlemskap">§ 3</a>, 1. ledd bestemmer.
            Oppgjør i forbindelse med overdragelsen skjer mellom partene og er
            klubben uvedkommende.
          </p>
          <li>Konkurs eller død</li>
          <p>
            Kommer et medlem under konkursbehandling eller avgår ved døden, skal
            hans andel beregnes på grunnlag av siste omsetningskurs før konkurs-
            eller dødsdagen og utbetales boet. Boet kan, om de ønsker det,
            opprettholde medlemskapet frem til første årsskifte hvorpå uttreden
            skjer på ordinær måte.
          </p>
          <li>Eksklusjon</li>
          <p>
            Et medlem kan ekskluderes dersom 2/3 av samtlige medlemmer i
            medlemsmøtet stemmer for dette. Eksklusjonsforslaget må bekjentgjøres
            og begrunnes minst to uker før det skal behandles.
            Eksklusjon må begrunnes i alvorlige forsømmelser eller gjentatte brudd
            på forpliktelser etter disse vedtekter eller andre forhold som på
            lignende måte skader klubbens interesser. Eksklusjonen er effektiv fra
            den dag den besluttes.
          </p>
          <p>
            Ekskludert medlems andel fastsettes som det laveste av følgende to beløp:
          </p>
          <ol type='a'>
            <li>medlemmets innbetalinger (startinnskudd og månedlige sparebeløp, ref. <a href="#kapital">§ 4</a>) eller</li>
            <li>90% av medlemmets andel vurdert slik denne paragraf bestemmer for
              uttreden. Uttredelsesdato tilsvarende eksklusjonsdato. Ekskludert
              medlems andel utbetales innen to uker.</li>
          </ol>
          <p>
            Begrunnes eksklusjonen i manglende innbetaling av det månedlige
            sparebeløp eller i at klubben på annen måte har et økonomisk krav på
            vedkommende, skal klubbens tilgodehavende fratrekkes andelen ved
            utbetaling.
          </p>
          <li>Inntreden</li>
          <p>
            Nytt medlem kan tre inn etter bestemmelsene i <a href="#medlemskap">§ 3</a>, 1. ledd, ref <a href="#organisasjon">§ 6.3</a>, 5. ledd.
          </p>
          <p>
            Det nye medlem foretar innbetaling som tilsvarer andelens verdi i h.h.t.
            klubbens regnskaper pr. 31. desember. Inntreden skjer pr. 1. januar.
            Andelen(e)s verdi skal innbetales umiddelbart etter avholdt årsmøte,
            senest innen utgangen av januar.
          </p>
          <p>
            Inntreden kan skje i løpet av året som følge av uttreden i h.h.t. denne
            paragrafs punkt 1, siste avsnitt.
          </p>
          <li>Kostnader</li>
          <p>
            Kostnader direkte knyttet til uttreden, eksklusjon og inntreden dekkes
            av det uttredende, ekskluderte eller inntredende medlem.
          </p>
        </ol>
        <h6 id="delvisuttreden">§ 12 DELVIS UTTREDEN OG INNTREDEN</h6>
        <ol>
          <li>Delvis uttreden</li>
          <p>
            Et medlem kan kreve seg utløst for en eller flere av sine andeler, jfr.
            dog <a href="#medlemskap">§ 3</a>, 2. ledd. Ved delvis uttreden gjelder de samme frister og
            beregningsmåter som for uttreden, jfr. <a href="#uttreden">§ 11</a>.
          </p>
          <li>Delvis inntreden</li>
          <p>
            Et medlem kan tre inn med nye andeler (jfr. dog <a href="#medlemskap">§ 3</a>, 2. ledd) hver år 1.
            januar ved å innbetale andelen(e)s verdi i h.h.t. klubbens regnskaper
            pr. 31. desember det foregående år. Delvis inntreden krever ikke
            særskilt godkjennelse, men må skriftlig meddeles lederen innen 1.
            desember. Andelen(e)s verdi skal innbetales umiddelbart etter avholdt
            årsmøte, senest innen utgangen av april.
          </p>
          <li>Kostnader</li>
          <p>
            Kostnader direkte knyttet til delvis inn- eller uttreden dekkes av det
            delvis uttredende eller delvis inntredende medlem.
          </p>
        </ol>
        <h6 id="opploesning">§ 13 OPPLØSNING</h6>
        <p>
          Oppløsning av klubben skal skje dersom minst 2/3 av samtlige medlemmer
          stemmer for det på et årsmøte der dette er satt på dagsorden i
          innkallelsen.
        </p>
        <hr></hr>
        <p>
          Sist endret: 22. desember 2021 - Bjørnar Næss<br />
          Vedlegg 1 - Investeringsstrategi<br />
          Vedlegg 2 - Medlemsoversikt<br />
        </p>
      </div>
    );
  }

}

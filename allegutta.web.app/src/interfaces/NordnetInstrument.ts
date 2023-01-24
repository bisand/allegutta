export interface NordnetInstrument {
    mifid2_category: number;
    price_type?: string;
    tradables?: Record<string, unknown>;
    instrument_id: number;
    asset_class?: string;
    instrument_type?: string;
    instrument_group_type?: string;
    currency?: string;
    multiplier: number;
    pawn_percentage: number;
    margin_percentage: number;
    symbol?: string;
    isin_code?: string;
    sector?: string;
    sector_group?: string;
    name?: string;
}
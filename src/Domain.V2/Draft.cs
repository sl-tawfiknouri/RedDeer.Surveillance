namespace Domain.V2
{


    // what else what else....
    // btw are we going to enforce TYPES with this i.e. Trader is a type of trader not just string?
    // would reduce bug problems
    // pain in the neck for general programming though
    // true but we can then have a TraderValidator which we use on input validation? make it a bit more OO

    // ok so we will go nutty on the typing, lets hope nobody hates us for it
    // yes yes...so what IS it we're passing around? the order! which has underneath it the trades und transactions, da.

    // Trader/PM/Broker
    // Vs counter party (?)
    
    // so a trade is an instrument plus some people plus some environmental factors i.e. prices
    // hmm and a transaction is really the trade within a trade

    // and what are the environmental factors? it is the Market at (T) 
    // so trade = instrument + market(T)

    // true but we wouldn't really model that? is the environment factor the trade data itself or a sub object?
    // I think it's OK just to list prices on the trade and fck the market(T) idea

    // alright so what are we going to roll with?
    // we need a big CSV class to read this raw data into
    
    // we then need a series of parsers + validators
    // parse + validate in one step? i.e. parser = validator + mapping (?)

    // once we have it we create the order[] and on that will be the trades and on that will be transactions
    // so the output of all of this is the order[]
    // and we will have a saving class capable of persisting those order []streams to the database

        // equity what? instrument?
    // who cares about an equity instrument? well it's the market data (duh) <- sooooo we dont care! not yet anyways lets see what's up
    // that's interesting...what else is there to the instrument beyond its identifiers? =)

    // sooooooooooo what is a market
    // it can be an exchange
    // dark pool
    // OTC
    // market type columns ????
    // I think so .....

    // maybe an easier approach is to get that CSV file type?
    // the means of a trade changes with each instrument type (?)

    public class Market
    {
    }
    
    public class Order
    {
    }

    public class Trade
    {
    }

    public class OptionTrade
    {
    }

    public class EquityTrade
    {
    }

    public class CreditTrade
    {
    }

    public class Transaction
    {
    }

    public class EquityTransaction
    {
    }
}

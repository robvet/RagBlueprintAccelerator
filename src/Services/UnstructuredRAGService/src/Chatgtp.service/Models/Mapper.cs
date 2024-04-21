using Shared.New.DTOs;

namespace UnstructuredRAG.Service.Models
{
    /// <summary>
    /// Map Entity Objects to Dto Objects
    public class Mapper
    {
        public static List<MessageDto> MapToMessagesToDto(List<Message> messageEntity)
        {
            var messageDtos = new List<MessageDto>();

            foreach (var item in messageEntity)
            {
                messageDtos.Add(new MessageDto
                {
                    Id = item.Id,
                    Type = item.Type,
                    SessionId = item.SessionId,
                    TimeStamp = item.TimeStamp,
                    Sender = item.Sender,
                    Tokens = item.Tokens,
                    Text = item.Text
                });
            }

            return messageDtos;
        }

        public static List<SessionDto> MapToSessionsToDto(List<Session> sessionEntity)
        {
            var sessionDtos = new List<SessionDto>();
            foreach (var item in sessionEntity)
            {
                sessionDtos.Add(new SessionDto
                {
                    SessionId = item.SessionId,
                    Id = item.Id,
                    Type = item.Type,
                    TokensUsed = item.TokensUsed,
                    Name = item.Name,
                    FrequencyPenalty = item.FrequencyPenalty,
                    NucluesSampling = item.NucluesSampling,
                    PresencePenalty = item.PresencePenalty,
                    PrivatePrompt = item.PrivatePrompt,
                    Temperature = item.Temperature,
                    TokenMax = item.TokenMax,
                    Messages = MapToMessagesToDto(item.Messages)
                });
            }
            return sessionDtos;
        }

        public static SessionDto MapToSessionToDto(Session sessionEntity)
        {
            return new SessionDto
            {
                SessionId = sessionEntity.SessionId,
                Id = sessionEntity.Id,
                Type = sessionEntity.Type,
                TokensUsed = sessionEntity.TokensUsed,
                Name = sessionEntity.Name,
                Messages = MapToMessagesToDto(sessionEntity.Messages)
            };
        }


        public static SummaryDto MapSummaryToDto(Summary summaryEntity)
        {
            return new SummaryDto
            {
                Name = summaryEntity.Name
            };
        }


        //public static BasketDto MapToBasketDto(Domain.Entities.Basket basketEntityEntity)
        //{
        //    // Transform Single BasketEntity to BasketDto
        //    var basketDto = new BasketDto
        //    {
        //        BasketId = basketEntityEntity.BasketId,
        //        ItemCount = basketEntityEntity.ItemCount,
        //        BuyerID = basketEntityEntity.BuyerId,
        //        Processed = basketEntityEntity.Processed
        //    };

        //    foreach (var item in basketEntityEntity.Items)
        //    {
        //        basketDto.CartItems.Add(new BasketItemDto
        //        {
        //            BasketParentId = item.BasketParentId,
        //            ProductId = item.ProductId,
        //            Title = item.Title,
        //            Artist = item.Artist,
        //            Genre = item.Genre,
        //            QuanityOrdered = item.Quantity,
        //            //Price = decimal.Parse(item.UnitPrice) * item.Quantity,
        //            Price = TransformUnitPrice(item.UnitPrice, item.Quantity),
        //            Condition = item.Condition,
        //            Status = item.Status,
        //            Medium = item.Medium,
        //            DateCreated = item.DateCreated
        //        }
        //    );

        //        basketDto.CartTotal = basketDto.CartItems.Sum(x => x.Price);
        //    }

        //    return basketDto;
        //}

        //public static List<BasketDto> MapToBasketDto(IEnumerable<Domain.Entities.Basket> baskets)
        //{
        //    var basketDtos = new List<BasketDto>();

        //    // Transform Collection of BasketEntity items to BasketDto
        //    foreach (var basket in baskets)
        //    {
        //        if (basket.BasketId != Guid.Empty)
        //        {  
        //            var basketDto = new BasketDto
        //            {
        //                BasketId = basket.BasketId,
        //                ItemCount = basket.ItemCount,
        //                BuyerID = basket.BuyerId,
        //                Processed = basket.Processed
        //            };

        //            foreach (var item in basket.Items)
        //            {
        //                basketDto.CartItems.Add(new BasketItemDto
        //                {
        //                    BasketParentId = item.BasketParentId,
        //                    ProductId = item.ProductId,
        //                    Title = item.Title,
        //                    Artist = item.Artist,
        //                    Genre = item.Genre,
        //                    QuanityOrdered = item.Quantity,
        //                    Price = TransformUnitPrice(item.UnitPrice, item.Quantity),
        //                    Condition = item.Condition,
        //                    Status = item.Status,
        //                    Medium = item.Medium,
        //                    DateCreated = item.DateCreated
        //                });
        //            }

        //            basketDto.CartTotal = basketDto.CartItems.Sum(x => x.Price * x.QuanityOrdered);
        //            basketDtos.Add(basketDto);
        //        }
        //    }

        //    return basketDtos;
        //}




        //public static BasketSummaryDto MapToBasketSummaryDto(BasketEntity genericEntities)
        //{
        //    // Transform BasketEntity to BasketDto
        //    var basketSummaryDto = new BasketSummaryDto
        //    {
        //        BasketId = genericEntities.BasketId,
        //        ItemCount = genericEntities.Items.Count,
        //        Processed = genericEntities.Processed,
        //        //basketSummaryDto.ProductNames = genericEntities.Items.SelectMany(x => x.Title).ToString();
        //        ProductNames = string.Join("\n", genericEntities.Items.Select(c => c.Title).Distinct())
        //    };

        //    return basketSummaryDto;
        //}

        //    public static BasketSummaryDto MapToBasketSummaryDto(Generic genericEntity)
        //    {
        //        // Transform BasketEntity to BasketDto
        //        var basketSummaryDto = new BasketSummaryDto
        //        {
        //            BasketId = genericEntity.BasketId,
        //            ItemCount = genericEntity.ItemCount,
        //            Processed = genericEntity.Processed
        //            //basketSummaryDto.ProductNames = genericEntities.Items.SelectMany(x => x.Title).ToString();
        //            //ProductNames = string.Join("\n", genericEntities.Items.Select(c => c.Title).Distinct())
        //        };

        //        return basketSummaryDto;
        //    }

        //    public static GenericDto MapToGenericEntitySummaryDto(List<Generic> genericEntities)
        //    {
        //        var genericEntityDto = new GenericDto();

        //        {

        //            foreach(Generic genericEntity in genericEntities) 
        //            {
        //                if (genericEntity.BasketId == Guid.Empty)
        //                {
        //                    genericEntityDto.Products.Add(new GenericSummaryDto
        //                    {
        //                        ProductId = genericEntity.ProductId,
        //                        Title = genericEntity.Title,
        //                        Artist = genericEntity.Artist,

        //                        BasketId = genericEntity.BasketId,
        //                        Processed = genericEntity.Processed,
        //                        ItemCount = genericEntity.ItemCount,
        //                    });
        //                }
        //                else
        //                {
        //                    genericEntityDto.Baskets.Add(new GenericSummaryDto
        //                    {
        //                        ProductId = genericEntity.ProductId,
        //                        Title = genericEntity.Title,
        //                        Artist = genericEntity.Artist,

        //                        BasketId = genericEntity.BasketId,
        //                        Processed = genericEntity.Processed,
        //                        ItemCount = genericEntity.ItemCount,
        //                    });
        //                }
        //            };
        //        };

        //        return genericEntityDto;
        //    }

        //    private static decimal TransformUnitPrice(string priceAsString, int quantity)
        //    {
        //        var success = decimal.TryParse(priceAsString, out var price);
        //        return success == true ? price * quantity : 0;
        //    }
        //}
    }
}
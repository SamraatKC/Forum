using Forum.Common;
using Forum.Data;
using Forum.Models.DataModels;
using Forum.Models.ViewModels;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Forum.Service
{
    public class ChannelService
    {
        ForumDbx db;
        IOptions<AppSettings> appSettings;
        public ChannelService(IOptions<AppSettings> _appSettings)
        {
            appSettings = _appSettings;
            db = new ForumDbx(_appSettings);
        }
        public async Task<bool> AddNewChannel(ChannelViewModel channelCustomerViewModel)
        {
            Channel channel = new Channel()
            {
                Title = channelCustomerViewModel.Title,
                Description = channelCustomerViewModel.Description,
                ChannelIconURL = channelCustomerViewModel.ChannelIconURL,
                DisplayOrder = channelCustomerViewModel.DisplayOrder,
                ParentChannel = channelCustomerViewModel.ParentChannel,
                
            };
            await db.Channels.AddAsync(channel);
            await db.SaveChangesAsync();
            return true;
        }
    }
}

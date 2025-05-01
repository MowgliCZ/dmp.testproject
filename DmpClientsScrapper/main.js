import { PlaywrightCrawler, Dataset } from 'crawlee';

async function main() {
    let allClients = [];

    const crawler = new PlaywrightCrawler({
        async requestHandler({ page }) {
            
            await page.waitForSelector('.list-item-wrapper.list-item-reference', { timeout: 30000 });
                        
            const clients = await page.$$eval('.list-item-wrapper.list-item-reference', items =>
                items.map(item => ({
                    Name: item.querySelector('.list-item-heading')?.textContent?.trim() || '',
                    Annotation: item.querySelector('.list-item-anotation')?.textContent?.trim() || '',
                    Content: item.querySelector('.overlay-txt')?.textContent?.trim() || '',
                    Link: item.querySelector('.overlay-link a')?.href || '',
                    Image: item.querySelector('.list-item-image')?.getAttribute('data-src') || ''
                }))
            );
            allClients.push(...clients);
                        
            const page2Link = await page.$('li.num:not(.active) a[href*="p.Page=2"]');
            if (page2Link) {
                await Promise.all([
                    page.waitForNavigation({ waitUntil: 'networkidle' }),
                    page2Link.click()
                ]);

                const clients2 = await page.$$eval('.list-item-wrapper.list-item-reference', items =>
                    items.map(item => ({
                        Name: item.querySelector('.list-item-heading')?.textContent?.trim() || '',
                        Annotation: item.querySelector('.list-item-anotation')?.textContent?.trim() || '',
                        Content: item.querySelector('.overlay-txt')?.textContent?.trim() || '',
                        Link: item.querySelector('.overlay-link a')?.href || '',
                        Image: item.querySelector('.list-item-image')?.getAttribute('data-src') || ''
                    }))
                );
                allClients.push(...clients2);
            }
        }
    });

    await crawler.run(['https://www.dmpublishing.cz/en/references']);

    await Dataset.pushData({
        TotalItems: allClients.length,
        Clients: allClients
    });
}

main();

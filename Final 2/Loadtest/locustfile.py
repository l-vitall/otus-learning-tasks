from locust import HttpUser, task, between
import faker

fake = faker.Faker()

class CommonUser(HttpUser):

    @task(10)
    def get_product_id(self):
        with self.client.get('/caloriesrecords/' + fake.uuid4(), catch_response=True) as response:
            if response.status_code == 404:
                response.success()
        
    @task(1)
    def search(self):
        self.client.get('/caloriesrecords?search=%20numberOfCalories%20gt%20' + str(fake.random_int(30, 1000)))

    wait_time = between(5, 15)
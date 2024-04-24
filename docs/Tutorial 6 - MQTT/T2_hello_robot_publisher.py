import time

from compas_eve import Message
from compas_eve import Publisher
from compas_eve import Subscriber
from compas_eve import Topic
from compas_eve.mqtt import MqttTransport

topic = Topic("/hellodaniela", Message)
server = MqttTransport("broker.hivemq.com")
msg = Message(result=f"Hello daniela")
print("/hellodaniela")
Publisher(topic, transport=server).publish(msg)

while True:
    time.sleep(1)


# for i in range(20):
#     msg = Message(result=f"Hello daniela #{i}")
#     print(f"Publishing message: {msg.result}")
#     Publisher(topic, transport=server).publish(msg)
#     time.sleep(3)
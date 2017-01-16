//
//  FFIAPManager.h
//  FlowFree
//
//  Created by Tran Truong Giang on 8/4/14.
//  Copyright (c) 2014 Apportable. All rights reserved.
//

#import "RMStore.h"
#import "RMStoreUserDefaultsPersistence.h"

#define sIAPMgr [FFIAPManager shareInstance]

@interface FFIAPManager : NSObject

+ (instancetype)shareInstance;

@property (nonatomic,readonly,strong) RMStore *store;
@property (nonatomic,readonly,strong) RMStoreUserDefaultsPersistence *persistence;

- (void)restoreTransactionsOnSuccess:(void (^)())successBlock
                             failure:(void (^)(NSError *error))failureBlock;

- (void)addPayment:(NSString*)productIdentifier
           success:(void (^)(SKPaymentTransaction *transaction))successBlock
           failure:(void (^)(SKPaymentTransaction *transaction, NSError *error))failureBlock;

- (BOOL)didPurchaseAnyProduct;

@end
